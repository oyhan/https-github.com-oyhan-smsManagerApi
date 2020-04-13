using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PSYCO.SmsManager.ApplicationConfig;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Services.Sms;

namespace PSYCO.SmsManager.Controllers
{
    public class CreditController: BaseController
    {
        private IAppRepository<TransactionModel, int> _transActionRepo;
        private AppSettings _settings;
        private ISmsService _smsService;
        private AppDbContext _db;


        public CreditController(IAppRepository<TransactionModel,int> transactionRepo 
            ,IOptionsSnapshot<AppSettings> options,
            ISmsService smsService,
            AppDbContext db)
        {
            _settings = options.Value;
            _transActionRepo = transactionRepo;
            _smsService = smsService;
            _db = db;
        }
        [HttpGet]

        public ActionResult Migrate()
        {
            var migrations = _db.Database.GetPendingMigrations();
            _db.Database.Migrate();
            return Ok(migrations);
        }

        [HttpPost]
        public async Task<ActionResult> AddCredit(TransactionViewModel vmodel)
        {
            using (var tr = _db.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                try
                {
                    var currenBallenc = _smsService.CurrentBallance(vmodel.UserId);
                    var noSMS = vmodel.Amount / _smsService.GetSmsTariff().Rate;
                    var transAction = new TransactionModel()
                    {

                        Amount = vmodel.Amount,
                        SmsBallance = currenBallenc.SmsBallance+noSMS,
                        CurrentBallance = currenBallenc.CurrentBallance+vmodel.Amount,
                        Description = vmodel.Description,
                        UserId = vmodel.UserId
                    };
                    await _transActionRepo.AddAsync(transAction);
                    await _transActionRepo.ApplyChangesAsync();
                    tr.Commit();

                    return Ok();
                }
                catch (Exception ex)
                {
                    tr.Rollback();

                  return  HandleException(ex);
                }
            }
            
        }




    }


    public class TransactionViewModel
    {
        public string UserId { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }

    }
}
