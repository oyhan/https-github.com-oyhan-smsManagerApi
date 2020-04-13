using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PSYCO.Common.Interfaces;
using PSYCO.Common.Repository;
using PSYCO.Common.SyncFusion.UrlAdaptor;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Helper;
using PSYCO.SmsManager.Tracking;

namespace PSYCO.SmsManager.Services.Sms
{
    public class SmsService : ISmsService
    {
        private AppDbContext _db;
        public SentSmsTracker SentSms { get; set; }
        public FailedSms FailedSms { get; set; }

        private IAppRepository<SendSmsModel, Guid> _repository;

        public DbContext _dbContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SmsService(AppDbContext db, IAppRepository<SendSmsModel, Guid> repository)
        {
            _db = db;
            SentSms = new SentSmsTracker(db);
            FailedSms = new FailedSms(db);
            _repository = repository;
        }
        public int CalculateMessageCost(string message)
        {
            var smsPrice = GetSmsTariff().Rate;
            var smsCount = GetMessageCount(message);



            return (int)(smsCount * (double)smsPrice);

        }
        public bool HasEnoughCredit(TransactionModel balance, int messageCost)
        {
            return balance != null && balance.CurrentBallance >= messageCost;
        }


        public bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        public int GetMessageCount(string message)
        {

            var length = message.ToCharArray().Length;
            var smsCount = 0d;
            if (length > 1 && length <= 70)
            {
                smsCount = 1;
            }
            else if (length > 70 && length <= 134)
            {
                smsCount = 2;
            }
            else
            {

                //در نظر گرفتن 3 کاراکتر به ازای هر اس ام اس به منظور یو اچ دی
                smsCount = Math.Ceiling((message.ToCharArray().Length - 134) / 67d) + 2;
            }

            return (int)smsCount;
        }

        public SmsTarrifModel GetSmsTariff()
        {
            var now = DateTime.Now;
            var tariff = _db.Tariffs
                .OrderByDescending(t => t.StartTime) // <-- order by start date descending 
                .FirstOrDefault(t => t.StartTime <= now);// <-- take the most fresh price policy
            if (tariff == null)
            {
                throw new Exception("No sms tariff registered! ");
            }

            return tariff;

        }

        public TransactionModel CurrentBallance(string userId)
        {
            var user = _db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new Exception("invalid userid");
            }

            var lastTransAction = _db.Transactions.Where(t => t.UserId == userId).OrderByDescending(c => c.CreatedDate).FirstOrDefault();
            if (lastTransAction == null)
            {
                throw new Exception("no transaction");

            }
            return lastTransAction;
        }

        //public IObservable<int> SentSms()
        //{
        // return  Observable.Create<int>(obsever =>

        //        OnSubscript(obsever)
        //    );

        //}

        private Action OnSubscript(IObserver<int> observer)
        {
            observer.OnNext(_db.SentSms.Count(s => s.IsSent));
            return () => { };
        }

        public Task<IReadOnlyList<SendSmsModel>> Where(Expression<Func<SendSmsModel, bool>> expression)
        {
            return _repository.Where(expression);
        }

        public Task<SendSmsModel> GetByIdAsync(Guid id)
        {
            return _repository.GetByIdAsync(id);

        }

        public Task<SendSmsModel> GetByIdAsync(Guid id, Expression<Func<SendSmsModel, object>> includes)
        {
            return _repository.GetByIdAsync(id, includes);

        }

        public Task<SendSmsModel> GetByIdAsync(Guid id, params ISpecification<SendSmsModel>[] spec)
        {
            return _repository.GetByIdAsync(id, spec);

        }

        public Task<IReadOnlyList<SendSmsModel>> GetAllAsync()
        {
            return _repository.GetAllAsync();

        }

        public Task<IReadOnlyList<SendSmsModel>> ListAsync(params ISpecification<SendSmsModel>[] spec)
        {
            return _repository.ListAsync(spec);


        }

        public Task<SendSmsModel> AddAsync(SendSmsModel entity)
        {
            return _repository.AddAsync(entity);

        }

        public Task UpdateAsync(SendSmsModel entity)
        {
            return _repository.UpdateAsync(entity);
        }

        public Task DeleteAsync(SendSmsModel entity)
        {
            return _repository.DeleteAsync(entity);

        }

        public Task DeleteByIdAsync(Guid id)
        {
            return _repository.DeleteByIdAsync(id);

        }

        public Task<int> CountAsync(params ISpecification<SendSmsModel>[] spec)
        {
            return _repository.CountAsync(spec);
        }

        public Task<int> ApplyChangesAsync()
        {
            return _repository.ApplyChangesAsync();
        }
        

    }

    public class ListSmsSpecification : SyncFusionSpecification<SendSmsModel, int>
    {
        public ListSmsSpecification(UrlAdaptorRequest<SendSmsModel, int> model)
        {

            MakeQuery(model);

        }
    }

    public class ListSmsByUserIdSpecification : BaseSpecification<SendSmsModel , int>
    {
        public ListSmsByUserIdSpecification(string userId ) : base(s=>s.UserId ==userId)
        {

        }
    }

    public class ListSmsByUserIdWithHolderSpecification : BaseSpecification<SendSmsModel, int>
    {
        //public ListSmsByUserIdSpecification(string userId,string holder) : base(s => s.UserId == userId)
        //{

        //}
    }

 
}
