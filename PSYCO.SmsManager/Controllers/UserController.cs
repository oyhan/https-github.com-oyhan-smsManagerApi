using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PSYCO.Common.Repository;
using PSYCO.Common.SyncFusion.UrlAdaptor;
using PSYCO.SmsManager.ApplicationConfig;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Helper;
using PSYCO.SmsManager.Services.Sms;
using PSYCO.SmsManager.Services.Transaction;
using PSYCO.SmsManager.Services.User;
using static PSYCO.SmsManager.Controllers.AccountController;

namespace PSYCO.SmsManager.Controllers
{
    //[Authorize(Roles = "Holder")]
    public class UserController : BaseController
    {





        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ISmsService _smsService;
        private readonly IAppRepository<TransactionModel, int> _transactionService;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
            ISmsService smsService,
            IUserService userService,
            IAppRepository<TransactionModel,int> transactionService


           )
        {
            _userService = userService;

            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _smsService = smsService;
            _transactionService = transactionService;
        }


        /// <summary>
        /// Adds a new user 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Add(RegisterDto model)

        {
            try
            {

                var transaction = new TransactionModel()
                {
                    Amount = model.InitialBallance,
                    SmsBallance = model.InitialBallance / _smsService.GetSmsTariff().Rate,
                    CurrentBallance = model.InitialBallance,
                    Description = "Initial Credit",

                };
                var user = new ApplicationUser()
                {
                    UserName = model.Username,
                    IsActive = model.IsActive,
                    HolderId = User.Id()

                };
                user.Transactions.Add(transaction);

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {


                    return Ok();
                    // user = await _userManager.FindByEmailAsync(model.Username);
                    ////if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    ////{
                    ////    var confirmEmailResult = await _userManager.SendEmailConfirmationAsync(user);
                    ////    Log.Logger.Information("emailsent");


                    ////    return Ok("یک ایمیل جهت تایید به ایمیل شما ارسال شد");


                    ////}
                    //await _signInManager.SignInAsync(user, false);


                    //return new { AccessToken = GenerateJwtToken(model.Username, user) };
                }

                return BadRequest(result.GetErrors());
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }



        }
        /// <summary>
        /// Lists all users registered by you.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {

            try
            {
                var holderId = User.Id();
                var users = ( _userService.ListUser(u => u.HolderId == holderId));
                return Ok();
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]

        public async Task<ActionResult> Get(UrlAdaptorRequest<UserListViewModel, string> model)
        {

            try
            {
                
                var users = (_userService.Query(model));
                return Ok(users);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet]
        [Route("{userId}")]
       public async Task<ActionResult> ListSms(string userId,int from ,int to)
        {
            try
            {
                var holder = User.Id();

                var smsList = await _smsService.ListAsync(new ListUserSmsSpecification(userId, from, to));
                return Ok(smsList);


            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpPost]
        [Route("{userId}")]
        public async Task<ActionResult> ListSms(string userId, UrlAdaptorRequest<SendSmsModel,int> model)
        {
            try
            {
                var holder = User.Id();
                if (true)
                {

                }
                var smsList = await _smsService.ListAsync(new ListSmsByUserIdSpecification(userId)
                    ,new ListSmsSpecification(model)
                    );
                var count = await _smsService.CountAsync(new ListSmsByUserIdSpecification(userId)
                    , new ListSmsSpecification(model));
                var result = new UrlAdaptorResponse<SendSmsModel>()
                {

                    count = count,
                    result = smsList
                };
                return Ok(result);


            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpPost]
        [Route("{userId}")]
        public async Task<ActionResult> ListTransactions(string userId, UrlAdaptorRequest<TransactionModel, int> model)
        {
            try
            {
                var holder = User.Id();
                if (true)
                {

                }
                var transactionList = await _transactionService.ListAsync(new ListTransactionByUserIdSpecification(userId)
                    , new ListTransactionSpecification(model)
                    );
                var count = await _transactionService.CountAsync(new ListTransactionByUserIdSpecification(userId)
                    , new ListTransactionSpecification(model));
                var result = new UrlAdaptorResponse<TransactionModel>()
                {

                    count = count,
                    result = transactionList
                };
                return Ok(result);


            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet]
        public async Task<ActionResult> Delete(string userId)
        {
            try
            {
                var holder = User.Id();
                var user = _userManager.Users.FirstOrDefault(u=>u.Id==userId);
                if (user==null)
                {
                    return NotFound($"User {userId} not found");

                }
                if (user.HolderId !=holder && !User.IsInRole(AppConstants.ADMIN))
                {
                    return Forbid("you do not have permission ");
                }
                var deleteResult = await _userManager.DeleteAsync(user);
                if (deleteResult.Succeeded)
                {
                    return Ok();
                }
                return StatusCode(500, deleteResult.GetErrors());

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }





    }


    public class ListUserSmsSpecification : BaseSpecification<SendSmsModel, Guid>
    {
        public ListUserSmsSpecification(string userId,int from ,int to) 
            :base(s=>s.UserId==userId )
        {
            ApplyPaging(from, to);
            ApplyOrderByDescending(c => c.CreatedDate);
                
        }

    }

}