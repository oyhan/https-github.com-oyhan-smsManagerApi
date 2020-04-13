using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PSYCO.Common.BaseModels;

namespace PSYCO.SmsManager.DomainObjects
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; }
        public List<TransactionModel> Transactions { get; set; } = new List<TransactionModel>();


        public string HolderId { get; set; }
        public ApplicationUser Holder { get; set; }

    }
}
