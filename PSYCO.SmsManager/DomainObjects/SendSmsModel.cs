using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.Common.BaseModels;

namespace PSYCO.SmsManager.DomainObjects
{
    public class SendSmsModel:BaseModel<Guid>
    {

        public int DeliveryStatus { get; set; }
        /// <summary>
        /// توضیحات در صورت بروز خطا یا هر چیز دیگر
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// شماره گیرنده
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// شناسه پیامک که فرستنده پیام تولید کرده است
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// تعداد پیامک حساب شده
        /// </summary>
        public int SmsCount { get; set; }
        /// <summary>
        /// محتوای پیامک
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// آیا یونیکد می باشد؟
        /// </summary>
        public bool IsUnicode { get; set; }
        /// <summary>
        /// میزان موجودی بعد از ارسال پیامک
        /// </summary>
        public bool IsSent { get; set; }
        public TransactionModel Transaction { get; set; }
        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public SmsTarrifModel Tariff { get; set; }
        public int TariffId { get; set; }
    }
}
