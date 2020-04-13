using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PSYCO.Common.SyncFusion.UrlAdaptor;
using PSYCO.SmsManager.ApplicationConfig;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Helper;
using PSYCO.SmsManager.Services.Sms;

namespace PSYCO.SmsManager.Controllers
{
    public class TariffController : BaseController
    {
        private AppSettings _settings;
        private IAppRepository<SmsTarrifModel, int> _tariffService;


        public TariffController( IOptionsSnapshot<AppSettings> options,
            IAppRepository<SmsTarrifModel, int> tariffService
           )
        {
            _settings = options.Value;
            _tariffService = tariffService;
        }
     
        [HttpPost]
        public async Task<ActionResult> List(UrlAdaptorRequest<SmsTarrifModel, int> model)
        {
            try
            {

                var tariffList = await _tariffService.ListAsync(new SyncFusionSpecification<SmsTarrifModel, int>(model)
                    );
                var count = await _tariffService.CountAsync(new SyncFusionSpecification<SmsTarrifModel, int>(model));
                var result = new UrlAdaptorResponse<SmsTarrifModel>()
                {

                    count = count,
                    result = tariffList
                };
                return Ok(result);


            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult> New(SmsTariffNewViewModel vmodel)
        {
            try
            {

                var model = new SmsTarrifModel()
                {

                    Rate = vmodel.Rate,
                    StartTime = PersianDateTime.Parse(vmodel.StartTime)
                };



                var tariff = await _tariffService.AddAsync(model);

                return Ok(tariff);


            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }


    public class SmsTariffNewViewModel
    {
        public int Rate { get; set; }
        public string StartTime { get; set; }

    }
}
