using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PSYCO.Common.BaseModels;
using PSYCO.Common.Interfaces;
using PSYCO.Common.Repository;

namespace PSYCO.SmsManager.Data
{
    public class AppRepository<T, TId> : EfAsyncRepository<T,TId>  ,IAppRepository<T, TId> where T : BaseModel<TId> 
    {
        public AppRepository(AppDbContext dbContext) : base(dbContext)
        {
        }


        public async Task<IReadOnlyList<T>> Where(Expression<Func<T, bool>> expression)
        {
            return await ListAsync( new GenericSpec<T,TId>(expression));
        }

        
    }


    public class GenericSpec<T,TId> : BaseSpecification<T,TId> where T : BaseModel<TId>
    {
        public GenericSpec(Expression<Func<T, bool>> criteria) : base(criteria)
        {
        }
    }
}
