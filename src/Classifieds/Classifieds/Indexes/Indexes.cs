using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Classifieds.Models;
using Raven.Client.Indexes;

namespace Classifieds
{

    public class CategorySearch : AbstractIndexCreationTask<Item, CategorySearch.Result>
    {
        public class Result
        {
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public string Fourth { get; set; }
        }

        public CategorySearch()
        {
            Map =  CategoryMap.TheMap;
        }
    }

    public class CategoryMap
    {
        public static Expression<Func<IEnumerable<Item>, IEnumerable>> TheMap
        {
            get
            {
                return items =>
                       from item in items
                       select new
                                  {
                                      First = item.Categories.Count > 0 ? item.Categories[0] : "",
                                      Second = item.Categories.Count > 1 ? item.Categories[1] : "",
                                      Third = item.Categories.Count > 2 ? item.Categories[2] : "",
                                      Fourth = item.Categories.Count > 3 ? item.Categories[3] : ""
                                  };
            }
        }
    }
}