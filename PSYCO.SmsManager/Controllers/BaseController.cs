using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PSYCO.SmsManager.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseController : Controller
    {
        [ApiExplorerSettings(IgnoreApi =true)]
        public ActionResult HandleException(Exception ex)
        {
            if (ex is DbUpdateException)
            {
                return StatusCode(500, "این آیتم به دلیل وابستگی در سیستم قابل حذف نمی باشد");
            }
            return StatusCode(500, ex.ToString());

        }
    }
}
