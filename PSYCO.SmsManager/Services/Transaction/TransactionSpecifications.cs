using PSYCO.Common.Repository;
using PSYCO.Common.SyncFusion.UrlAdaptor;
using PSYCO.SmsManager.DomainObjects;
using PSYCO.SmsManager.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSYCO.SmsManager.Services.Transaction
{
    public class ListTransactionByUserIdSpecification : BaseSpecification<TransactionModel, int>
    {
        public ListTransactionByUserIdSpecification(string userId) : base(s => s.UserId == userId)
        {

        }
    }


    public class ListTransactionSpecification : SyncFusionSpecification<TransactionModel, int>
    {
        public ListTransactionSpecification(UrlAdaptorRequest<TransactionModel, int> model)
        {

            MakeQuery(model);

        }
    }

}
