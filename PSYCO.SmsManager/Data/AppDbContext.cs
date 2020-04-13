using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PSYCO.Common.Repository;
using PSYCO.SmsManager.DomainObjects;

namespace PSYCO.SmsManager.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {

       
        public AppDbContext(DbContextOptions options) :base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.SeedIdentity();

            //SendSms configurations
            builder.Entity<SendSmsModel>().Property(s => s.Recipient)
                .HasMaxLength(11);
            builder.Entity<SendSmsModel>().Property(s => s.MessageId)
                .HasMaxLength(15);
            builder.Entity<SendSmsModel>().Property(s => s.Description)
               .HasMaxLength(300);
            builder.Entity<SendSmsModel>().Property(s => s.SmsCount)
              .HasMaxLength(1000);

            //Transaction configuration 
            builder.Entity<TransactionModel>().Property(s => s.Description)
             .HasMaxLength(300);


            base.OnModelCreating(builder);
        }

        public DbSet<SendSmsModel> SentSms { get; set; }
        public DbSet<SmsTarrifModel> Tariffs { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }

    }
}
