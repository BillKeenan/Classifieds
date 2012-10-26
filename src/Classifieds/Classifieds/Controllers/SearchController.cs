using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Classifieds.Indexes;
using Classifieds.services.data;

namespace Classifieds.Controllers
{
    public class SearchController : Controller
    {
        //
        // GET: /Search/

        public ActionResult Index()
        {
            using(var sess = RavenStore.Instance("classifieds").OpenSession())
            {
                var result = sess.Query<CategoryFirstMapReduce.Result,CategoryFirstMapReduce>();
                return View(result);
            }
        }

    }
}
