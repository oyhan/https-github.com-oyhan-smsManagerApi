using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PSYCO.Common.Repository;
using PSYCO.SmsManager.DomainObjects;

namespace PSYCO.SmsManager.Services.Tariff
{
    public class TariffRepository :EfAsyncRepository<SmsTarrifModel,int>
    {
        public TariffRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
