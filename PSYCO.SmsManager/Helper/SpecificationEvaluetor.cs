using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PSYCO.Common.BaseModels;
using PSYCO.Common.Interfaces;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace PSYCO.SmsManager.Helper
{
    public class SpecificationEvaluator<T, TId> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, params ISpecification<T>[] specifications)
        {
            var query = inputQuery;

            // modify the IQueryable using the specification's criteria expression


            foreach (var specification in specifications)
            {
                if (specification.Criteria != null)
                {
                    query = query.Where(specification.Criteria);
                }
                else if (specification.CriteriaString != null)
                {
                    query = query.Where(specification.CriteriaString);

                }
                // Includes all expression-based includes
                query = specification.Includes.Aggregate(query,
                    (current, include) => current.Include(include));
                // Include any string-based include statements
                query = specification.IncludeStrings.Aggregate(query,
                    (current, include) => current.Include(include));


                // Apply ordering if expressions are set
                if (specification.OrderBy != null)
                {
                    query = query.OrderBy(specification.OrderBy);
                }
                else if (specification.OrderByDescending != null)
                {
                    query = query.OrderBy(specification.OrderByDescending);
                }
                else if (specification.OrderByString.Count>0)
                {
                    query = query.OrderBy(specification.OrderByString[0]);
                    foreach (var orderby in specification.OrderByString.Skip(1))
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy(orderby);
                    }
                }
                if (specification.OrderByDescendingString.Count > 0)
                {
                    query = query.OrderBy($"{specification.OrderByDescendingString[0]} descending");

                    foreach (var orderby in specification.OrderByDescendingString.Skip(1))
                    {
                        query = ((IOrderedQueryable<T>)query).ThenBy($"{orderby} descending");
                    }
                }

                if (specification.GroupBy != null)
                {
                    query = query.GroupBy(specification.GroupBy).Select(x=>x.First());
                }
                else if (specification.GroupByString != null)
                {
                    query =(IQueryable<T>) query.GroupBy(specification.GroupByString)
                        .Select("first()");
                }

                // Apply paging if enabled
                if (specification.IsPagingEnabled)
                {
                    query = query.Skip(specification.Skip)
                        .Take(specification.Take);
                }
            }



            return query;
        }
    }
}
