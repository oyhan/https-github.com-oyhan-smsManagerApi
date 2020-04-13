using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.SmsManager.Services;

namespace PSYCO.SmsManager.ApiModels
{
    public class SmsApiModel:SmsModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Content { get; set; }
        public string Recipient { get; set; }
    }
}
