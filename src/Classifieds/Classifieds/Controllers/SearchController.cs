using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Classifieds.Indexes;
using Classifieds.Models;
using Classifieds.ViewModel;
using Classifieds.services;
using Classifieds.services.data;

namespace Classifieds.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index()
        {
            using (var sess = RavenStore.Instance("classifieds").OpenSession())
            {
                var result = sess.Query<CategoryFirstMapReduce.Result, CategoryFirstMapReduce>().ToList();
                return Return("Index",result.Select(ViewModelCreator.BuildCategoryCount));
            }
        }

        private ActionResult Return(string theView,Object theObject)
        {
            if (Request.AcceptTypes.Any(type => type.ToLower() == "application/json"))
            {
                Response.ContentType = "application/json";
                return Json(theObject,JsonRequestBehavior.AllowGet);
            }
            return View(theView, theObject);
        }

        public ActionResult Category(string first, string second, string third, string fourth)
        {

            var items = new List<Item>();
            using (var sess = RavenStore.Instance("classifieds").OpenSession())
            {
                    IEnumerable<CategoryCount> returnOb =null;

                if (!String.IsNullOrEmpty(first))
                {
                    var obQuery=
                        sess.Advanced.LuceneQuery<Item>("CategorySearch").WhereEquals("First", first);

                    if (string.IsNullOrEmpty(second))
                    {
                        var result =
                            sess.Query<CategorySecondMapReduce.Result, CategorySecondMapReduce>().Where(
                                x => x.First == first).ToList();
                        returnOb = result.Select(ViewModelCreator.BuildCategoryCount);
                    }
                    else
                    {
                        obQuery = obQuery.AndAlso().WhereEquals("Second", second);
                        if (string.IsNullOrEmpty(third))
                        {
                            var result =
                                sess.Query<CategoryThirdMapReduce.Result, CategoryThirdMapReduce>().Where(
                                    x => x.First == first && x.Second == second).ToList();
                            returnOb = result.Select(ViewModelCreator.BuildCategoryCount);
                        }
                        else
                        {
                            obQuery = obQuery.AndAlso().WhereEquals("Third", third);

                            if (string.IsNullOrEmpty(fourth))
                            {
                                var result =
                                    sess.Query<CategoryFourthMapReduce.Result, CategoryFourthMapReduce>().Where(
                                        x => x.First == first && x.Second == second && x.Third == third).ToList();
                                returnOb = result.Select(x=>ViewModelCreator.BuildCategoryCount(x,3));
                            }else
                            {
                                var result =
                                    sess.Query<CategoryFourthMapReduce.Result, CategoryFourthMapReduce>().Where(
                                        x => x.First == first && x.Second == second && x.Third == third && x.Fourth == fourth).ToList();

                                returnOb = result.Select(x=>ViewModelCreator.BuildCategoryCount(x,4));

                                //special case for the end of the line

                                obQuery = obQuery.AndAlso().WhereEquals("Fourth", fourth);

                            }
                        }
                    }

                    items = obQuery.ToList();
                    ViewBag.Results = items;
                    return Return("Category",returnOb);
                }
                else
                {
                    throw new ArgumentException("No Category present");
                }
            }
        }
    }
}
