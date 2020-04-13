using Microsoft.AspNetCore.Mvc;
using PSYCO.SmsManager.ApiModels;
using PSYCO.SmsManager.ResponseMessages;
using PSYCO.SmsManager.Services;
using System;
using System.Threading.Tasks;

namespace PSYCO.SmsManager.Controllers
{
    public class SmsController : BaseController
    {
        private ISendSmsService<SmsApiModel, SendSmsResponseModel> _sender;

        public SmsController(ISendSmsService<SmsApiModel, SendSmsResponseModel> sender)
        {
            _sender = sender;
        }

        [HttpPost]
        public ActionResult SendSms(SmsApiModel sms)
        {
            try
            {
                var respons = _sender.Send(sms);
                return Ok(respons);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.ToString());
            }
        }


        [Route("/api")]
        [Produces("application/xml")]
        [HttpGet]

        public async Task<ActionResult<response>> Ozeki(string action, string username, string password, string recipient,
                                                     string messagetype, string messagedata, string msgID)
        {


            switch (action.ToLower())
            {
                case "sendmessage":
                    var smsModel = new SmsApiModel()
                    {
                        Content = messagedata,
                        Recipient = recipient,
                        Password = password,
                        Username = username
                    };
                    var result = await _sender.Send(smsModel);

                    var respons = new response()
                    {
                        action = action,
                        data = new ResponseMessages.Data()
                        {
                            acceptreport = new AcceptReport()
                            {
                                messageid = result.MessageId.ToString(),
                                statuscode = result.StatusCode == SmsResponseStatusCode.Sent ? 0 : 2
                            }
                        }
                    };
                    return Ok(respons);

                case "getmessagebyid":

                    var deliveryReport = await _sender.DeliveryReport(msgID);
                    var deliveryrespons = new response()
                    {
                        action = action,
                        data = new ResponseMessages.Data()
                        {
                            state = deliveryReport.StatusCode !=SmsResponseStatusCode.InvalidMessageId? 1 : -1
                        }
                    };


                    return Ok(deliveryrespons);
            }

            return BadRequest("invalid request");

        }
    }
}
