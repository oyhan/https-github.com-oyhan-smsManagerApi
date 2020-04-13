using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using PSYCO.Infrastructure.Communications.Services.Sms.Ozeki;

namespace PSYCO.SmsManager.Test
{
    public class SmsService
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SmsUrl { get; set; }

        public SmsService(string smsUrl,string username, string password)
        {

            UserName = username;
            Password = password;
            SmsUrl = smsUrl;
        }
        public ISmsResult Send(string number , string text)
        {
            try
            {
                //http://sms.magfa.com
                //magfaHttpServiceservice =Enqueue&domain=magfa&username=pooyan&password=elXlPHtAWYjbopOT&from=%2B98300073658&to=%2B989138609215&message=سلام
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = new TimeSpan(0, 0, 5);
                    //httpClient.BaseAddress = new Uri(SmsServiceConstants.DomainSmsServiceGsm);
                    httpClient.BaseAddress = new Uri(SmsUrl);
                    //                httpClient.BaseAddress = new Uri("http://sms.magfa.com");
                    var url = "api?action=sendmessage&username=" + UserName+ "&password=" +
                              Password + "&recipient=" + number +
                              "&messagetype=SMS:TEXT&messagedata=" +
                              text;
                    var result = httpClient.GetStreamAsync(url).Result;

                    //                var result = httpClient.GetAsync("/magfaHttpService?service=Enqueue&domain=magfa&username=pooyan&password=elXlPHtAWYjbopOT&from=%2B98300073658&to=%2B"+ sms.Numbers[0] + "&message="+ sms.Text).Result;
                    //                var xmlDoc = new XmlDocument();
                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "response";
                    xRoot.IsNullable = true;
                    var serializer = new XmlSerializer(typeof(SmsResponse), xRoot);
                    var response = (SmsResponse) serializer.Deserialize(result);
                    if (response.data.acceptreport != null && response.data.acceptreport.statuscode == 0)
                    {
                        var messageid = response.data.acceptreport.messageid;
                        Thread.Sleep(100);
                        if (DeliveryReport(messageid))
                        {
                            return new SmsResult()
                            {
                                IsSent = true
                            };
                        }

                        return new SmsResult()
                        {
                            IsSent = false
                        };
                    }
                    else
                    {
                        return new SmsResult()
                        {
                            IsSent = false,
                            //TrackingID = result.
                        };
                    }
                }

            }
            catch (Exception e)
            {
                return new SmsResult()
                {
                    IsSent = false,
                };
            }
        }



        private bool DeliveryReport(string messageid)
        {

            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = new TimeSpan(0, 0, 5);
                    //httpClient.BaseAddress = new Uri(SmsServiceConstants.DomainSmsServiceGsm);
                    httpClient.BaseAddress = new Uri(SmsUrl);
                    var url = $"api?action=getmessagebyid&username={UserName}&password=" +
                              $"{Password}&msgID={messageid}";

                    var result = httpClient.GetStreamAsync(url).Result;

                    XmlRootAttribute xRoot = new XmlRootAttribute();
                    xRoot.ElementName = "response";
                    xRoot.IsNullable = true;
                    var serializer = new XmlSerializer(typeof(SmsResponse), xRoot);
                    var response = (SmsResponse) serializer.Deserialize(result);

                    if (response.data.state == 1 || response.data.state == 10)
                    {
                        return true;
                    }
                    else return false;

                }
            }
            catch (Exception e)
            {

                return false;
            }

        }
    }
}

public class SmsResult : ISmsResult
{
    public bool IsSent { get; set; }
}

public interface ISmsResult
{
     bool IsSent { get; set; }
}