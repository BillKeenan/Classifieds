using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Classifieds.Indexes;
using Classifieds.ViewModel;

namespace Classifieds.services
{
    public class ViewModelCreator
    {
        public static CategoryCount BuildCategoryCount(CategoryFirstMapReduce.Result search)
        {
            return new CategoryCount
                         {
                             Categories = new List<string>{search.First},
                             Count = search.Count,
                             Depth=1
                         };
        }

        public static CategoryCount BuildCategoryCount(CategorySecondMapReduce.Result search)
        {
            return new CategoryCount
            {
                Categories =  new List<string>{ search.First ,search.Second},
                Count = search.Count,
                Depth=1
            };
        }

        public static CategoryCount BuildCategoryCount(CategoryThirdMapReduce.Result search)
        {
            return new CategoryCount
            {
                Categories =  new List<string>{ search.First, search.Second,search.Third },
                Count = search.Count,
                Depth=2
            };
        }

        public static CategoryCount BuildCategoryCount(CategoryFourthMapReduce.Result search,int depth)
        {
            return new CategoryCount
            {
                Categories =  new List<string>{ search.First, search.Second, search.Third ,search.Fourth},
                Depth = depth,
                Count = search.Count
            };
        }
    }
}