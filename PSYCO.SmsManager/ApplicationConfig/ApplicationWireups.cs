using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetify;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PSYCO.Common.Interfaces;
using PSYCO.SmsManager.ApiModels;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Services;
using PSYCO.SmsManager.Services.SendSmsServiceProviders.Magfa;
using PSYCO.SmsManager.Services.Sms;
using PSYCO.SmsManager.Services.User;
using Swashbuckle.AspNetCore.Swagger;

namespace PSYCO.SmsManager.ApplicationConfig
{
    public static class ApplicationConfigurations
    {

        public static void CongfigureApp(this IServiceCollection services , IConfiguration Configuration)
        {

            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));

            services.AddDbContext<AppDbContext>(
                opt => opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
            );
            services.AddSignalR();
            services.AddDotNetify();
            services.AddIdentity<ApplicationUser, IdentityRole>(o=>o.Password = new PasswordOptions()
                {
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false,
                    
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders().
                AddUserManager<UserManager<ApplicationUser>>();



            //configure api bihavior
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            //add swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sms Manager API", Version = "v1" });
            });



            //config app repository
            services.AddScoped(typeof(IAppRepository<,>),typeof(AppRepository<,>));

            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IUserService, UserService>();


            //sms sender provider
            services.AddScoped(typeof(ISendSmsService<SmsApiModel, SendSmsResponseModel>),typeof(MagfaSendSmsProvider)) ;

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims

            var key = Encoding.ASCII.GetBytes(AppConstants.SecretKey);
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;

                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            //using Hangfire
            var hangfireOptions = new SqlServerStorageOptions
            {
                PrepareSchemaIfNecessary = true
            };

            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection"), hangfireOptions));
            services.AddHangfireServer();
        }

        public static void EnsureLastMigrationApplyed<TDbContext>(this IApplicationBuilder app) where TDbContext : DbContext
        {
            //ensure last migration applyed.
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TDbContext>();
                context.Database.Migrate();
            }

        }
        public static void UseAppConfigs(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseWebSockets();
            app.UseDotNetify();
            app.UseSignalR(routes => routes.MapDotNetifyHub());

            app.UseSwagger();

            //             Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            //             specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });



            //Hangfire Dashboard
            app.UseHangfireDashboard(options: new DashboardOptions()
            {
                Authorization = null
            });
            app.UseHangfireServer(new BackgroundJobServerOptions()
            {
                WorkerCount = 1,

            });
        }
    }
}
