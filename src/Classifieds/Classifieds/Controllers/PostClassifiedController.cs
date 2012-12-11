using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Classifieds.Models;
using Classifieds.services.data;

namespace Classifieds.Controllers
{
    public class PostClassifiedController : BaseController
    {
        //
        // GET: /PostClassified/
        [Authorize]
        public ActionResult Index()
        {
            using (var sess = RavenStore.Instance("templates").OpenSession())
            {
                var templates = sess.Query<Template>().Select(x => x.Name);
                return View(templates);
            }

           
        }

        //
        // GET: /PostClassified/Details/5

        public ActionResult Template(string id)
        {
            using (var sess = RavenStore.Instance("templates").OpenSession())
            {
                var theTemplate = sess.Query<Template>().FirstOrDefault(x => x.Name == id);
                return View( theTemplate);
            }

            
        }

       

        //
        // POST: /PostClassified/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /PostClassified/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /PostClassified/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /PostClassified/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /PostClassified/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
