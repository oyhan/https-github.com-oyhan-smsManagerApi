using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using PSYCO.SmsManager.ApiModels;
using PSYCO.SmsManager.ApplicationConfig;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PSYCO.Common.Interfaces;
using PSYCO.SmsManager.Services.Sms;

namespace PSYCO.SmsManager.Services.SendSmsServiceProviders.Magfa
{
    public class MagfaSendSmsProvider : ISendSmsService<SmsApiModel, SendSmsResponseModel>
    {
        private AppDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        private AppSettings _settings;
        private IAppRepository<TransactionModel, int> _transactionRepo;
        private IAppRepository<SendSmsModel, Guid> _smsRepo;
        private ISmsService _smsService;

        public MagfaSendSmsProvider(AppDbContext db, UserManager<ApplicationUser> userManager
            , IOptionsSnapshot<AppSettings> options
            , IAppRepository<TransactionModel,int> transactionRepo,
            IAppRepository<SendSmsModel,Guid> smsRepo,
            ISmsService smsService
            )
        {
            _settings = options.Value;
            _db = db;
            _userManager = userManager;
            _smsRepo = smsRepo;
            _transactionRepo = transactionRepo;
            _smsService = smsService;
        }
        public async Task<SendSmsResponseModel> Send(SmsApiModel apiModel)
        {
            var user = await _userManager.FindByNameAsync(apiModel.Username);
            if (user == null)
            {
                return new SendSmsResponseModel()
                {
                    StatusCode = SmsResponseStatusCode.UserNotFound
                };
            }

            var isValidCredential = await _userManager.CheckPasswordAsync(user, apiModel.Password);
            if (!isValidCredential)
            {
                return new SendSmsResponseModel()
                {
                    StatusCode = SmsResponseStatusCode.InvalidPassword
                };
            }

            if (!user.IsActive)
            {
                return GetRespond(SmsResponseStatusCode.UserInActive, null);
            }
            var messageCost = _smsService.CalculateMessageCost(apiModel.Content);

            var sms = new DomainObjects.SendSmsModel()
            {
                Content = apiModel.Content,
                SmsCount = _smsService.GetMessageCount(apiModel.Content),
                UserId = user.Id,
                IsUnicode = _smsService.ContainsUnicodeCharacter(apiModel.Content),
                TariffId = _smsService.GetSmsTariff().Id,
                Recipient = apiModel.Recipient

            };

            var balance = _smsService.CurrentBallance(user.Id);
            bool hasEnoughCredit = _smsService.HasEnoughCredit(balance, messageCost);

            if (balance == null)
            {
                balance = new TransactionModel()
                {
                    Amount = messageCost,
                    CurrentBallance = 0,
                    SendSms = sms,
                    UserId = user.Id,
                    Description = "No ballance record found",
                    SmsBallance = 0,
                };
                sms.Transaction = balance;

                await _smsRepo.AddAsync(sms);
                await _smsRepo.ApplyChangesAsync();
                return new SendSmsResponseModel()
                {
                    Message = "No ballance record found",
                    MessageId = sms.Id,
                    StatusCode = SmsResponseStatusCode.NotEnoughCredit
                };
            }


            if (!hasEnoughCredit)
            {
                var newTransaction = new TransactionModel()
                {
                    Amount = messageCost,
                    CurrentBallance = balance.CurrentBallance,
                    SendSms = sms,
                    UserId = user.Id,
                    Description = "Not enough credit",
                    SmsBallance = balance.CurrentBallance,

                };
                sms.Transaction = newTransaction;
                await _smsRepo.AddAsync(sms);
                await _smsRepo.ApplyChangesAsync();

                return new SendSmsResponseModel()
                {
                    MessageId = sms.Id,
                    StatusCode = SmsResponseStatusCode.NotEnoughCredit,
                };
            }

            //user has enough credits send sms
            //now send sms

            sms = Enqueue(sms);
            if (sms.IsSent)
            {
                var newTransaction = new TransactionModel()
                {
                    Amount = -messageCost,
                    CurrentBallance = balance.CurrentBallance - messageCost,
                    SendSms = sms,
                    UserId = user.Id,
                    SmsBallance = balance.SmsBallance - _smsService.GetMessageCount(sms.Content),

                };
                sms.Transaction = newTransaction;
                await _smsRepo.AddAsync(sms);
                await _smsRepo.ApplyChangesAsync();

                return GetRespond(SmsResponseStatusCode.Sent, sms);

            }

            return GetRespond(SmsResponseStatusCode.Sent, sms);





        }


        private async Task InsertSms(SendSmsModel sms)
        {
            var balance = _db.Transactions
                .OrderByDescending(t => t.CreatedDate).FirstOrDefault();
            var messageCost = _smsService.CalculateMessageCost(sms.Content);
            var newTransaction = new TransactionModel()
            {
                Amount = messageCost,
                CurrentBallance = balance.CurrentBallance - messageCost,
                SendSms = sms,
                UserId = sms.UserId,
                SmsBallance = balance.SmsBallance - _smsService.GetMessageCount(sms.Content),

            };
            sms.Transaction = newTransaction;
            await _smsRepo.AddAsync(sms);
            await _smsRepo.ApplyChangesAsync();
        }
        public async Task<SendSmsResponseModel> DeliveryReport(string messageId)
        {
            var message = (await _smsRepo.Where(m => m.Id == Guid.Parse(messageId))).FirstOrDefault();
            if (message == null)
            {
                return GetRespond(SmsResponseStatusCode.InvalidMessageId, message);
            }
            var requestHandler = new RequestHandler();
            String realStatusUrl = UrlGenerator.generateGetRealMessageStatusUrl(
                _settings.SmsProviderUsername,
                _settings.SmsProviderPassword,
                "magfa",
               message.MessageId
            );
            String realStatusResults = requestHandler.get(realStatusUrl);
            int returnValue_real = (int)double.Parse(realStatusResults);
            message.DeliveryStatus = returnValue_real;
            await _db.SaveChangesAsync();
            if (returnValue_real <= -1)
            {
                return GetRespond(SmsResponseStatusCode.InvalidMessageId, message);
            }
            else
            {
                return GetRespond(GetRealDeliveryStatus(returnValue_real), message);
            }


        }

        private SmsResponseStatusCode GetRealDeliveryStatus(int status)
        {
            switch (status)
            {
                case 1:
                    return SmsResponseStatusCode.DeliveredToPhone;
                case 2:
                    return SmsResponseStatusCode.NotDeliveredToPhone;

                case 8:
                    return SmsResponseStatusCode.DeliveredToNetwork;

                case 16:
                    return SmsResponseStatusCode.NotDeliveredToNetwork;

                case 0:
                    return SmsResponseStatusCode.NoDeliveryReportAvailable;
                default:
                    throw new Exception("Unknown delivery report code from magfa");
            }

        }
        private SendSmsModel Enqueue(DomainObjects.SendSmsModel smsmodel)
        {
            try
            {

                var requestHandler = new RequestHandler();
                String enqueueUrl = UrlGenerator.generateEnqueueUrl(
                    _settings.SmsProviderUsername,
                    _settings.SmsProviderPassword,
                    "magfa",
                    "98300073658",
                    smsmodel.Recipient,
                    smsmodel.Content,
                    "",
                    "",
                    ""
                );
                String messageId = requestHandler.get(enqueueUrl);
                long returnValue = long.Parse(messageId);
                if (returnValue <= ErrorCodes.MAX_VALUE)
                {
                    smsmodel.Description = ErrorCodes.getDescriptionForCode(returnValue);
                    return smsmodel;

                }
                else
                {
                    smsmodel.MessageId = messageId;
                    smsmodel.IsSent = true;
                    return smsmodel;
                }
            }
            catch (Exception exception)
            {
                smsmodel.Description = exception.ToString();
                return smsmodel;
            }

        }


       
      
        private SendSmsResponseModel GetRespond(SmsResponseStatusCode status, SendSmsModel sms)
        {
            if (sms == null)
            {
                return new SendSmsResponseModel()
                {
                    Message = GetStatusMessage(status),
                    StatusCode = status,

                };
            }

            if (status == SmsResponseStatusCode.Sent)
            {
                _smsService.SentSms.Publish(_db.SentSms.Count(s=>s.IsSent));

            }
            else
            {
                _smsService.FailedSms.Publish(_db.SentSms.Count(s=>!s.IsSent));

            }
            return new SendSmsResponseModel()
            {
                Message = status == SmsResponseStatusCode.Failed ? sms.Description : GetStatusMessage(status),
                StatusCode = status,
                MessageId = sms.Id
            };
        }

        private string GetStatusMessage(SmsResponseStatusCode status)
        {

            switch (status)
            {
                case SmsResponseStatusCode.Sent:
                    return "پیامک با موفقیت ارسال شد";
                case SmsResponseStatusCode.Failed:
                    return "ارسال پیامک با خطا روبرو شد";
                case SmsResponseStatusCode.UserNotFound:
                    return "کاربری با این مشخصات یافت نشد";
                case SmsResponseStatusCode.NotEnoughCredit:
                    return "موجودی نا کافی";
                case SmsResponseStatusCode.UserInActive:
                    return "کاربر غیرفعال می باشد";
                case SmsResponseStatusCode.InvalidPassword:
                    return "کلمه عبور صحیح نیست";
                case SmsResponseStatusCode.InvalidMessageId:
                    return "شناسه پیامک معتبر نیست";
                case SmsResponseStatusCode.DeliveredToPhone:
                    return "تحویل شده به گوشی";
                case SmsResponseStatusCode.DeliveredToNetwork:
                    return "تحویل شده به مخابرات";
                case SmsResponseStatusCode.NotDeliveredToNetwork:
                    return "به مخابرات تحویل نشده";

                case SmsResponseStatusCode.NoDeliveryReportAvailable:
                    return "هیچ گزارشی در دسترس نیست";

                default:
                    throw new ArgumentOutOfRangeException(nameof(status), status, null);
            }
        }

    }
}
