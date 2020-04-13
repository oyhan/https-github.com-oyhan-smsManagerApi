using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.SmsManager.ApiModels;

namespace PSYCO.SmsManager.Services
{
    public interface ISendSmsService<TSendModel,TResponseModel> where  TSendModel : SmsModel where TResponseModel : SendSmsResponseModel
    {
        Task<TResponseModel> Send(TSendModel apiModel);
        Task<TResponseModel> DeliveryReport(string messageId);


    }
}
