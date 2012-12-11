using System.Collections.Generic;
using System.Text;

namespace Classifieds.ViewModel
{
    public class CategoryCount
    {
        public List<string> Categories;
        public int Count;

        public int Depth { get; set; }

        public string GetPath(int position)
        {
            var sb = new StringBuilder();
            for (int i = 0; (i < Categories.Count && i < position+1); i++)
            {
                if (i > 0)
                {
                    sb.Append("/");
                }
                sb.Append(Categories[i]);
                
            }

            return sb.ToString();
        }
    }
}