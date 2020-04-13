using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PSYCO.SmsManager.ApplicationConfig;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Helper;
using PSYCO.SmsManager.Services.Sms;
using PSYCO.SmsManager.Services.User;

namespace PSYCO.SmsManager.Controllers
{
    [AllowAnonymous]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AccountController : BaseController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ISmsService _smsService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
            ISmsService smsService,
            IUserService userService
            
           )
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _smsService = smsService;
            
        }

        [HttpPost]
        public async Task<object> Login([FromBody] LoginDto model)
        {
            try
            {

                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    return BadRequest("کاربری با چنین مشخصات یافت نشد");

                }
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);


                if (result.Succeeded)
                {

                    var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.Username);

                    return new
                    {
                        Name = appUser.UserName,
                        Roles = await _userManager.GetRolesAsync(appUser),
                        AccessToken = GenerateJwtToken(model.Username, appUser),
                        ExpireDate = DateTime.Now.AddDays(15)
                    };
                }

                return BadRequest("نام کاربری یا کلمه عبور اشتباه است");

            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }
        }

        [HttpGet]

        public async Task<bool> IsAuthenticated()
        {
            return await Task.FromResult(User.Identity.IsAuthenticated);
        }

      

        //[HttpPost]
        //public async Task<object> Register([FromBody] RegisterDto model)
        //{

        //    try
        //    {

        //        var result = await _userManager.CreateCustomerAsync(model);

        //        if (result.Succeeded)
        //        {
        //            Log.Logger.Information("ready to send email");
        //            var user = await _userManager.FindByEmailAsync(model.Username);
        //            if (_userManager.Options.SignIn.RequireConfirmedEmail)
        //            {
        //                Log.Logger.Information("ready to send email");
        //                var confirmEmailResult = await _userManager.SendEmailConfirmationAsync(user);
        //                Log.Logger.Information("emailsent");

        //                if (confirmEmailResult)
        //                {
        //                    return Ok("یک ایمیل جهت تایید به ایمیل شما ارسال شد");
        //                }

        //            }
        //            await _signInManager.SignInAsync(user, false);


        //            return new { AccessToken = GenerateJwtToken(model.Username, user) };
        //        }
        //        return BadRequest(result.GetErrors());
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Logger.Fatal(ex.ToString());

        //        throw;
        //    }

        //    throw new ApplicationException("UNKNOWN_ERROR");
        //}

        //[HttpGet]
        //public async Task<object> Confirm(string code, string uid)
        //{
        //    var user = await _userManager.FindByIdAsync(uid);
        //    if (user != null)
        //    {
        //        var confirmResult = await _userManager.ConfirmEmailAsync(user, code);
        //        if (confirmResult.Succeeded)
        //        {
        //            return Ok("ایمیل با موفقیت تایید شد");
        //        }
        //        return BadRequest("اطلاعات وارد شده غلط می باشد ");

        //    }
        //    return BadRequest("کد کاربر نا معتبر");

        //}


        [HttpPost]
        public async Task<object> RegisterUser([FromBody] RegisterDto model)
        {

            try
            {
               
                var transaction = new TransactionModel()
                {
                    Amount = model.InitialBallance,
                    SmsBallance = model.InitialBallance /_smsService.GetSmsTariff().Rate ,
                    CurrentBallance = model.InitialBallance,
                    Description = "Initial Credit",
                    
                };
                var user = new ApplicationUser()
                {
                    UserName    = model.Username,
                    IsActive =  model.IsActive,
                    
                };
                user.Transactions.Add(transaction);

                var result = await _userManager.CreateAsync(user,model.Password);
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
                return StatusCode(500, "خطایی رخ داده ");
                throw;
            }

            throw new ApplicationException("UNKNOWN_ERROR");
        }





        private object GenerateJwtToken(string email, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppConstants.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(15);

            var token = new JwtSecurityToken(
                null,
                null,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpGet]

        public async Task<ActionResult> GetAll()
        {
            try
            {
                return Ok( _userService.ListUser(null));
            }
            catch (Exception ex)
            {

                return HandleException(ex);
            }
        }

        public class LoginDto
        {
            [Required]
            public string Username { get; set; }

            [Required]
            public string Password { get; set; }

        }


        public class UserListViewModel
        {
            public string Id { get; set; }
            public string Username { get; set; }
            public int Ballance { get; set; }
            public bool IsActive { get; set; }
            public int SentSms { get; set; }
            public int SmsBallance { get; set; }
        }

        public class RegisterDto
        {
            [Required]
            public string Username { get; set; }
            [Required]
            public int InitialBallance { get; set; }

            public bool IsActive { get; set; }
           

            [Required]
            [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
            public string Password { get; set; }
        }
    }
}
