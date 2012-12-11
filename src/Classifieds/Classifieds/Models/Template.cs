using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Classifieds.Models
{
    public class Template
    {
        public Template()
        {
            CategoryDefs = new List<ListingProperty>();
            
        }
        public string Name { get; set; }
        public List<ListingProperty> CategoryDefs { get; set; }
    }


    public class ListingProperty
    {
        public ListingProperty()
        {
            Samples=new List<string>();
        }
        public enum CatType
        {
            Select,
            Int,
            Text
        }
        public enum Storage
        {
            Cat,Tag
        }

        public Storage PropType = Storage.Cat;
        public CatType CategoryType { get; set; }
        public int? MaxLength { get; set; }
        public List<String> Samples { get; set; }
        public string Name { get; set; }
    }

}