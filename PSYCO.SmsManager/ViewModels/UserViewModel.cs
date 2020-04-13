using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DotNetify;
using Microsoft.AspNetCore.Identity;
using PSYCO.SmsManager.Controllers;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Services.Sms;
using PSYCO.SmsManager.Services.User;

namespace PSYCO.SmsManager.ViewModels
{
    public class UserViewModel : BaseVM
    {
        private UserManager<ApplicationUser> _userManager;
        private ISmsService _smsService;
        private IUserService _userService;

        public  UserViewModel(UserManager<ApplicationUser> userManager,
            ISmsService smsService,
            IUserService userService
            )
        {
            _userManager = userManager;
            _smsService = smsService;
            _userService = userService;
            Users = userService.ListUser(null);

        }

        public List<AccountController.UserListViewModel> Users { get; set; } 
        public string  Result { get; set; }

        public void AddUser(AccountController.RegisterDto model)
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

            };
            user.Transactions.Add(transaction);

            var result =  _userManager.CreateAsync(user, model.Password).Result;
            if (result.Succeeded)
            {


                Result = "کاربر با موفقیت ثبت شد";
            }
        }



    }
}
