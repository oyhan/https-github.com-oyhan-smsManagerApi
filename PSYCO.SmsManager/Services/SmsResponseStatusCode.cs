using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSYCO.SmsManager.Services
{
    public enum SmsResponseStatusCode
    {
        Sent,
        Failed,
        UserNotFound,
        NotEnoughCredit,
        UserInActive,
        InvalidPassword,
        InvalidMessageId,
        DeliveredToPhone,
        DeliveredToNetwork,
        NotDeliveredToNetwork,
        NoDeliveryReportAvailable,
        NotDeliveredToPhone
    }
}
