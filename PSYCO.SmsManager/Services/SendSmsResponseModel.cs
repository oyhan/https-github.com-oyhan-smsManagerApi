using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSYCO.SmsManager.Services
{
    public  class SendSmsResponseModel
    {
        public Guid MessageId { get; set; }
        public SmsResponseStatusCode StatusCode { get; set; }
        public string Message { get; set; }

    }
}
