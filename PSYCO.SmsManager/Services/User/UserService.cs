using Microsoft.AspNetCore.Identity;
using PSYCO.Common.Repository;
using PSYCO.Common.SyncFusion.UrlAdaptor;
using PSYCO.SmsManager.Controllers;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Helper;
using PSYCO.SmsManager.Services.Sms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static PSYCO.SmsManager.Controllers.AccountController;

namespace PSYCO.SmsManager.Services.User
{
    public class UserService : IUserService
    {
        private UserManager<ApplicationUser> _userManager;
        private ISmsService _smsService;

        public UserService(UserManager<ApplicationUser> usermanage,
            ISmsService smsService)
        {
            _userManager = usermanage;
            _smsService = smsService;
        }


        public List<UserListViewModel> ListUser(Expression<Func<ApplicationUser, bool>> expression)
        {
            if (expression == null)
            {
                return _userManager.Users.ToList().Select(u => new UserListViewModel()
                {
                    Id = u.Id,
                    IsActive = u.IsActive,
                    Username = u.UserName,
                    Ballance = _smsService.CurrentBallance(u.Id).CurrentBallance,
                    SentSms = _smsService.Where(s => s.UserId == u.Id & s.IsSent).Result.Count,
                    SmsBallance = _smsService.CurrentBallance(u.Id).SmsBallance
                }).ToList();
            }
            return _userManager.Users.Where(expression).ToList().Select(u => new UserListViewModel()
            {
                Id = u.Id,
                IsActive = u.IsActive,
                Username = u.UserName,
                Ballance = _smsService.CurrentBallance(u.Id).CurrentBallance,
                SentSms = _smsService.Where(s => s.UserId == u.Id & s.IsSent).Result.Count,
                SmsBallance = _smsService.CurrentBallance(u.Id).SmsBallance
            }).ToList();
        }

        public UrlAdaptorResponse<UserListViewModel> Query(UrlAdaptorRequest<UserListViewModel, string> model)
        {
            try
            {
                var users = Common.Repository.SpecificationEvaluator<ApplicationUser, string>
               .GetQuery(_userManager.Users,false ,new ListUserSpecification(model));
                var count =  Common.Repository.SpecificationEvaluator<ApplicationUser, string>
             .GetQuery(_userManager.Users, true, new ListUserSpecification(model));
                return new UrlAdaptorResponse<UserListViewModel>()
                {
                    count = count.Count(),
                    result = users.ToList().Select(u => new UserListViewModel()
                    {
                        Id = u.Id,
                        IsActive = u.IsActive,
                        Username = u.UserName,
                        Ballance = _smsService.CurrentBallance(u.Id).CurrentBallance,
                        SentSms = _smsService.Where(s => s.UserId == u.Id & s.IsSent).Result.Count,
                        SmsBallance = _smsService.CurrentBallance(u.Id).SmsBallance
                    }).ToList()
                };
            }
            catch (Exception ex)
            {

                throw;
            }
          
           
        }
    }


    public class ListUserSpecification : SyncFusionSpecification<ApplicationUser, string>
    {
        public ListUserSpecification(UrlAdaptorRequest<UserListViewModel, string> model) 
        {

            MakeQuery(model);

        }
    }
}



