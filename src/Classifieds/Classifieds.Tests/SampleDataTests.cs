﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Classifieds.Models;
using Classifieds.services.data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raven.Abstractions.Data;

namespace Classifieds.Tests
{
    [TestClass]
    public class SampleDataTests
    {



        [TestMethod]
        public void CreateItems()
        {
            RavenStore.Instance("classifieds").DatabaseCommands.DeleteByIndex(
               "Raven/DocumentsByEntityName", new IndexQuery());


            using (var sess = RavenStore.Instance("classifieds").OpenSession())
            {

                var brands = new[] { "Ford", "Subaru", "Toyota" };
                var models = new[] { "coupe", "truck", "wagon" };
                var years = new[] { "1991", "1996", "2000", "2012" };


                foreach (var brand in brands)
                {
                    foreach (var year in years)
                    {
                        foreach (var model in models)
                        {
                            var categories = new List<String>{"Auto",brand, year, model};
                            var item = new Item
                            {
                                Description =
                                    string.Format(" A {0} {1}, the model is {2}", year, brand, model),
                                    Categories = categories
                            };

                            item.Tags.AddRange(new[] { model, year, brand });

                            sess.Store(item, string.Format("{0}/", String.Join("/",categories)));
                        }
                    }
                }

                sess.SaveChanges();

            }
        }


        [TestMethod]
        public void CreateIndexes()
        {
            var doc =RavenStore.Instance("classifieds");
            Raven.Client.Indexes.IndexCreation.CreateIndexes(typeof(FourthCategorySearch).Assembly, doc);

        }

        [TestMethod]
        public void GetBrands()
        {
            var categories = new List<Category>();
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out)); 
            using (var sess = RavenStore.Instance("classifieds").OpenSession())
            {
                var fords = from m in sess.Query<Item>() where m.Categories[0] == "Auto"  select m;
                Trace.WriteLine(String.Format("FOund:{0}",fords.Count()));
            }
        }

        public void CreateCategories()
        {
            var categories = new List<Category>();

            RavenStore.Instance("classifieds").DatabaseCommands.DeleteByIndex(
                "Raven/DocumentsByEntityName", new IndexQuery());

            
            using (var sess = RavenStore.Instance("classifieds").OpenSession())
            {

                var car = new Category { Name = "Autos" };
                sess.Store(car);

                var brands = new[] { "Ford", "Subaru", "Toyota" };
                var models = new[] { "coupe", "truck", "wagon" };
                var years = new[] { "1991", "1996", "2000", "2012" };


                foreach (var brand in brands)
                {
                    foreach (var year in years)
                    {
                        foreach (var model in models)
                        {
                            var item = new Item
                                           {
                                               Description =
                                                   string.Format(" A {0} {1}, the model is {2}", year, brand, model)
                                           };

                            item.Tags.AddRange(new[] {model, year, brand});

                            sess.Store(item, string.Format("{0}/", car.Code));
                        }
                    }
                }

                sess.SaveChanges();
                
            }
        }

        [TestMethod]
        public void LoadFords()
        {
            using (var sess = RavenStore.Instance("classifieds").OpenSession())
            {
                var cars = sess.Load<Item>().Where(x => x.Tags.Contains("Ford")).ToList();
                Assert.IsTrue(cars.Count > 0);
            }
        }

    }
}