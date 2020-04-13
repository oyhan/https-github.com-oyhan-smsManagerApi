using PSYCO.Common.Repository;
using PSYCO.Common.SyncFusion.UrlAdaptor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSYCO.SmsManager.Helper
{
    public class SyncFusionSpecification<T,TId> : BaseSpecification<T,TId>
    {
        public SyncFusionSpecification()
        {

        }
        public SyncFusionSpecification(UrlAdaptorRequest<T, TId> model)
        {
            MakeQuery(model);
        }


        protected void MakeQuery<TVModel>(UrlAdaptorRequest<TVModel, TId> model)
        {
            ApplyPaging(model.Skip, model.Take);

            //group by
            if (model.Group != null)
            {
                if (model.Group?.Count > 1)
                {
                    var groupByString = $"new (";

                    foreach (var groupBy in model.Group)
                    {
                        groupByString += groupBy + ",";

                    }
                    groupByString = groupByString.Remove(groupByString.LastIndexOf(','), 1);

                    groupByString += ")";
                    ApplyGroupBy(groupByString);

                }
                else ApplyGroupBy(model.Group[0]);
            }


            //order by
            if (model.Sorted != null)
            {
                foreach (var sortBy in model.Sorted)
                {

                    if (sortBy.Direction == "ascending")
                    {
                        ApplyOrderBy(sortBy.Name);
                    }
                    else
                        ApplyOrderByDescending(sortBy.Name);

                }

            }

            //Search criteria
            if (model.Search != null)
            {
                if (model.Search.Count > 0)
                {

                    var perdicate = "";
                    foreach (var searchItem in model.Search)
                    {
                        searchItem.Fields = searchItem.Fields.Where(f => typeof(T).GetProperties().Any(p => p.PropertyType == typeof(string) && p.Name.ToLower() == f.ToLower())).ToList();
                        var key = searchItem.Key;
                        perdicate += "(";
                        foreach (var field in searchItem.Fields)
                        {
                            perdicate += $"{field}.{searchItem.Operator}(\"{key}\") or ";
                        }
                        perdicate = perdicate.Remove(perdicate.LastIndexOf("or"), 2);
                        perdicate += ") and ";

                    }
                    perdicate = perdicate.Remove(perdicate.LastIndexOf("and"), 3);
                    AddCriteriaString(perdicate);
                }

            }
        }
    }


    
}
