using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Tracking;

namespace PSYCO.SmsManager.Services.Sms
{
    public interface ISmsService : IAppRepository<SendSmsModel,Guid> 
    {
        int CalculateMessageCost(string message);
         SentSmsTracker SentSms { get; set; }
         FailedSms FailedSms{ get; set; }
        bool HasEnoughCredit(TransactionModel balance, int messageCost);
        bool ContainsUnicodeCharacter(string input);
        int GetMessageCount(string message);
        SmsTarrifModel GetSmsTariff();
        TransactionModel CurrentBallance(string userId);
    }
}
