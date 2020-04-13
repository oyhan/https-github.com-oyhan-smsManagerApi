using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSYCO.Common.Interfaces;
using PSYCO.Infrastructure.Communications.Services.Sms.Ozeki;

namespace PSYCO.SmsManager.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
           var smsService = new SmsService("http://localhost:2594","Admin","Psyco123@465");
           var result = smsService.Send("09353469229", "”·«„");
           

        }


      
    }
}
