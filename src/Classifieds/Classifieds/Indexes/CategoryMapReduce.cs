using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classifieds.Models;
using Raven.Client.Indexes;

namespace Classifieds.Indexes
{
    public class CategoryFirstMapReduce : AbstractIndexCreationTask<Item, CategoryFirstMapReduce.Result>
    {
       public class Result
        {
            public string First { get; set; }
            public int Count { get; set; }
        }

        public CategoryFirstMapReduce(){
                Map=items =>
                       from item in items
                       select new
                                  {
                                      First = item.Categories.Count > 0 ? item.Categories[0] : "",
                                      Count=1
                                  };

            Reduce = results => from result in results
                                group result by result.First
                                into g
                                select new
                                           {
                                               First=g.Key,
                                               Count = g.Sum(x => x.Count)
                                           };


        }
    }
}