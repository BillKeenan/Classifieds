using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classifieds.Models;
using Raven.Client.Indexes;

namespace Classifieds.Indexes
{
    public class CategorySecondMapReduce : AbstractIndexCreationTask<Item, CategorySecondMapReduce.Result>
    {
       public class Result
        {
            public string First { get; set; }
            public string Second { get; set; }
            public int Count { get; set; }
        }

       public CategorySecondMapReduce()
       {
                Map=items =>
                       from item in items
                       select new
                                  {
                                      First = item.Categories.Count > 0 ? item.Categories[0] : "",
                                      Second = item.Categories.Count > 1 ? item.Categories[1] : "",
                                      Count=1
                                  };

            Reduce = results => from result in results
                                group result by new {result.First,result.Second}
                                into g
                                select new
                                           {
                                               First = g.Key.First,
                                               Second = g.Key.Second,
                                               Count = g.Sum(x => x.Count)
                                           };


        }
    }

    public class CategoryFirstMapReduce : AbstractIndexCreationTask<Item, CategoryFirstMapReduce.Result>
    {
        public class Result
        {
            public string First { get; set; }
            public int Count { get; set; }
        }

        public CategoryFirstMapReduce()
        {
            Map = items =>
                   from item in items
                   select new
                   {
                       First = item.Categories.Count > 0 ? item.Categories[0] : "",
                       Count = 1
                   };

            Reduce = results => from result in results
                                group result by result.First
                                    into g
                                    select new
                                    {
                                        First = g.Key,
                                        Count = g.Sum(x => x.Count)
                                    };


        }
    }

    public class CategoryThirdMapReduce : AbstractIndexCreationTask<Item, CategoryThirdMapReduce.Result>
    {
        public class Result
        {
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public int Count { get; set; }
        }

        public CategoryThirdMapReduce()
        {
            Map = items =>
                   from item in items
                   select new
                   {
                       First = item.Categories.Count > 0 ? item.Categories[0] : "",
                       Second = item.Categories.Count > 1 ? item.Categories[1] : "",
                       Third = item.Categories.Count > 2 ? item.Categories[2] : "",
                       Count = 1
                   };

            Reduce = results => from result in results
                                group result by new { result.First, result.Second ,result.Third}
                                    into g
                                    select new
                                    {
                                        g.Key.First,
                                        g.Key.Second,
                                        g.Key.Third,
                                        Count = g.Sum(x => x.Count)
                                    };


        }
    }


    public class CategoryFourthMapReduce : AbstractIndexCreationTask<Item, CategoryFourthMapReduce.Result>
    {
        public class Result
        {
            public string First { get; set; }
            public string Second { get; set; }
            public string Third { get; set; }
            public string Fourth { get; set; }
            public int Count { get; set; }
        }

        public CategoryFourthMapReduce()
        {
            Map = items =>
                   from item in items
                   select new
                   {
                       First = item.Categories.Count > 0 ? item.Categories[0] : "",
                       Second = item.Categories.Count > 1 ? item.Categories[1] : "",
                       Third = item.Categories.Count > 2 ? item.Categories[2] : "",
                       Fourth= item.Categories.Count > 2 ? item.Categories[3] : "",
                       Count = 1
                   };

            Reduce = results => from result in results
                                group result by new { result.First, result.Second, result.Third,result.Fourth }
                                    into g
                                    select new
                                    {
                                        g.Key.First,
                                        g.Key.Second,
                                        g.Key.Third,
                                        g.Key.Fourth,
                                        Count = g.Sum(x => x.Count)
                                    };


        }
    }
    
}