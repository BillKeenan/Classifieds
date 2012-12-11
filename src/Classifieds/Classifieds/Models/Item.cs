using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Classifieds.Models
{
    public class Item 
    {
        public Item()
        {
            Tags = new List<string>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Location { get; set; }

        public List<String> Tags { get; set; }

        public List<string> Categories { get; set; }

        public string Image { get; set; }
    }
}