using PSYCO.Common.SyncFusion.UrlAdaptor;
using PSYCO.SmsManager.Data;
using PSYCO.SmsManager.DomainObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static PSYCO.SmsManager.Controllers.AccountController;

namespace PSYCO.SmsManager.Services.User
{
    public interface IUserService 
    {
        List<UserListViewModel> ListUser(Expression<Func<ApplicationUser,bool>> expression);

        UrlAdaptorResponse<UserListViewModel> Query(UrlAdaptorRequest<UserListViewModel, string> model);

    }
}
