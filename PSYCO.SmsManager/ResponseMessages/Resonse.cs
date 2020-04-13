using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSYCO.SmsManager.ResponseMessages
{
    public class response
    {
        public string action { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public AcceptReport acceptreport { get; set; }
        public int state { get; set; }

    }

    public class AcceptReport
    {
        public int statuscode { get; set; }
        public string statusmessage { get; set; }
        public string messageid { get; set; }
    }
}
