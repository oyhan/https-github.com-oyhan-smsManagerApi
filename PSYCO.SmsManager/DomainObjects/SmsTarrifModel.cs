using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSYCO.Common.BaseModels;

namespace PSYCO.SmsManager.DomainObjects
{
    public class SmsTarrifModel : BaseModel<int>
    {
        public int Rate { get; set; }
        public DateTime StartTime { get; set; }
    }
}
