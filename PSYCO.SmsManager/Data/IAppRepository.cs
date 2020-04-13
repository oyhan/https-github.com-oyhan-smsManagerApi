using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PSYCO.Common.BaseModels;
using PSYCO.Common.Interfaces;

namespace PSYCO.SmsManager.Data
{
    public interface IAppRepository<T,TId> : IAsyncRepository<T,TId> where T : BaseModel<TId>
    {
        Task<IReadOnlyList<T>> Where(Expression<Func<T,bool>> expression);
    }
}
