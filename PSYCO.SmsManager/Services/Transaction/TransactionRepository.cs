using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PSYCO.Common.Repository;
using PSYCO.SmsManager.DomainObjects;

namespace PSYCO.SmsManager.Services.Transaction
{
    public class TransactionRepository: EfAsyncRepository<TransactionModel,int>
    {
        public TransactionRepository(DbContext dbContext) : base(dbContext)
        {
        }
    }
}
