using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace Classifieds
{
    public static class ImageHelper
    {
     

        public static object MediaHelper(string image)
        {
            var sb = new StringBuilder();
            sb.Append(ConfigurationManager.AppSettings["MediaPath"]);
            sb.Append(image);
            return new MvcHtmlString(sb.ToString());
        }

        public static object MediaHelper(string image, int width, int height, bool forceFit)
        {
            var sb = new StringBuilder();
            sb.Append(MediaHelper(image));
            sb.Append(string.Format("?width={0}&height={1}&force={2}", width, height,forceFit));
            return sb.ToString();
        }
    }

}
