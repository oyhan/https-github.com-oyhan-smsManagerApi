using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.Common.BaseModels;

namespace PSYCO.SmsManager.DomainObjects
{
    public class TransactionModel : BaseModel<int>
    {
        public TransactionModel()
        {
            CreatedDate = DateTime.Now;
        }
        /// <summary>
        /// مبلغ تراکنش
        /// </summary>
        public int Amount { get; set; }
        /// <summary>
        /// موجودی پیامک
        /// </summary>
        public int SmsBallance { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// مجموع  
        /// </summary>
        public int CurrentBallance { get; set; }

        public SendSmsModel SendSms { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        

    }
}
