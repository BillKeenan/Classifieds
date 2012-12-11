using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using Classifieds.services;

namespace Classifieds
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        private HttpContext _context;

        public ImageHandler()
        {

        }


        public void ProcessRequest(HttpContext context)
        {
            _context = context;
            try
            {
                var url = context.Request.RawUrl;


                var fi = (FileInfo)HttpRuntime.Cache.Get(url);

                if (fi == null || !fi.Exists)
                {

                    var parmas = url.Split(Char.Parse("/"), Char.Parse("?"));

                    var width = 0;
                    var height = 0;
                    var force = false;
                    if (parmas.Length > 3)
                    {
                        var queryParams = QueryHelper.SplitParams(parmas[3]);


                        //get width)
                        if (queryParams.ContainsKey("width"))
                        {
                            int.TryParse(queryParams["width"], out width);
                        }
                        //get width)
                        if (queryParams.ContainsKey("height"))
                        {
                            int.TryParse(queryParams["height"], out height);
                        }

                        if (queryParams.ContainsKey("force"))
                        {
                            bool.TryParse(queryParams["force"], out force);
                        }


                    }

                    //is it an allowed size
                    var config = ImageSizeSection.GetConfig();
                    if (config == null)
                    {
                        //no sizes set, return orig image
                        width = height = 0;
                    }
                    else
                    {
                        var imageSizes = config.ImageSizes;

                        var size = imageSizes.FirstOrDefault(x => x.Width == width && x.Height == height);

                        if (size == null)
                        {
                            //no matchin config, use orig
                            width = height = 0;
                        }
                    }
                    fi = HandleLocalImage(parmas[2],  width, height,force);
                }

                if (fi != null)
                {

                    HttpRuntime.Cache.Add(url, fi, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration,
                                          CacheItemPriority.Normal, null);
                    ReturnImage(fi, context);
                }
                else
                {
                    context.Response.StatusCode = 404;
                    return;
                }
            }
            catch (Exception e)
            {
                context.Response.StatusCode = 404;
            }
        }

        private FileInfo HandleLocalImage(string id, int width, int height,bool force)
        {

            var key = String.Format("Image{0}Cache", id);
            var resizedkey = String.Format("Image{0}Cache_w:{1}_h:{2}_f:{3}", id, width, height,force);


            FileInfo fi = null;

            var sizedFile = (string)HttpRuntime.Cache.Get(resizedkey);
            if (!String.IsNullOrEmpty(sizedFile) && File.Exists(sizedFile))
            {
                fi = new FileInfo(sizedFile);
            }
            else
            {

                var origFile = (string)HttpRuntime.Cache.Get(key);


                //get the original filepath
                if (String.IsNullOrEmpty(origFile))
                {
                    var filePath = GetImage(id);

                    if (!String.IsNullOrEmpty(filePath))
                    {
                        origFile = String.Format("{0}{1}", ConfigurationManager.AppSettings["ImagePath"], filePath);

                        HttpRuntime.Cache.Add(key, origFile, null, Cache.NoAbsoluteExpiration,
                                              Cache.NoSlidingExpiration,
                                              CacheItemPriority.Normal, null);


                    }

                    //does the file exist?
                    if (!String.IsNullOrEmpty(origFile))
                    {
                        if (!File.Exists(origFile))
                        {
                            NotFound();
                            return null;
                        }



                    }
                }
                if (origFile != null)
                {
                    fi = new FileInfo(origFile);
                }
                else
                {
                    return null;
                }

                if (!fi.Exists)
                {
                    
                }

                //build the sized url
                if (width > 0 || height > 0)
                {
                    var sized = String.Format(@"{0}\{1}", fi.DirectoryName, GetFileName(fi.Name, width, height, force));

                    //does it exist?
                    if (!File.Exists(sized))
                    {
                        //create it
                        fi = ResizeImage(width, height, fi, force);
                    }
                    else
                    {
                        fi = new FileInfo(sized);
                    }


                    HttpRuntime.Cache.Add(resizedkey, sized, null, Cache.NoAbsoluteExpiration,
                                          Cache.NoSlidingExpiration,
                                          CacheItemPriority.Normal, null);
                }

            }

            return fi;
        }

        private void NotFound()
        {
            _context.Response.StatusCode = 404;
        }

        public void ReturnImage(FileInfo file, HttpContext context)
        {
            var etag = Md5Util.GetMd5Hash(file.LastWriteTime.Ticks + file.FullName);

            context.Cache[file.FullName] = etag;

            switch (file.Extension.ToLower())
            {
                case ".jpg":
                    context.Response.ContentType = "image/jpeg";
                    break;
                case ".png":
                    context.Response.ContentType = "image/png";
                    break;
                case ".gif":
                    context.Response.ContentType = "image/gif";
                    break;
                default:
                    context.Response.ContentType = "image/jpeg";
                    break;
            }

            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.Cache.SetExpires(DateTime.Now.AddDays(1));
            context.Response.Cache.SetMaxAge(new TimeSpan(24, 0, 0));
            context.Response.Cache.SetETag(etag);
            context.Response.WriteFile(file.FullName);
        }

      
        public string GetImage(string id)
        {
            var sb = new StringBuilder();
            //ids are guids, split them up
            var path = id.Split(Char.Parse("-"));
            sb.Append(String.Join(@"\", path));
            sb.Append(@"\");
            sb.Append(id);
            sb.Append(".jpg");
            return sb.ToString();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }




        private static string GetFileName(string name, int width, int height, bool force)
        {
            var sb = new StringBuilder();

            sb.Append(width);
            sb.Append("_");
            sb.Append(height);
            sb.Append("_");
            sb.Append(force);
            sb.Append("_");
            sb.Append(name);
            
            return sb.ToString();
        }

        public static FileInfo ResizeImage(int width, int height, FileInfo fi, bool force)
        {
            if (width == 0 && height == 0)
            {
                return fi;
            }



            var newFileName = String.Format(@"{0}\{1}", fi.Directory, GetFileName(fi.Name, width, height,force));

            if (File.Exists(newFileName))
            {
                var newFi = new FileInfo(newFileName);
                return newFi;
            }

            FileInfo finalFI;
            //load up the image
            using (var image = Image.FromFile(fi.FullName))
            {


                var jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                var myEncoderParameters = new EncoderParameters(1);
                myEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);

                using (var fullsizeImage = Image.FromFile(fi.FullName))
                {
                    //get newSize

                    if (force)
                    {
                        // Prevent using images internal thumbnail
                        using (var newImage = ForceResizeImage(fullsizeImage, width, height))
                        {
                            // Save resized picture
                            newImage.Save(newFileName, jgpEncoder, myEncoderParameters);
                            newImage.Dispose();
                            finalFI = new FileInfo(newFileName);
                        }
                    }
                    else
                    {
                        var newSize = NewSize(fullsizeImage.Size, new Size(width, height));

                        // Prevent using images internal thumbnail
                        using (var newImage = ActualResizeImage(fullsizeImage, newSize.Width, newSize.Height))
                        {
                            // Save resized picture
                            newImage.Save(newFileName, jgpEncoder, myEncoderParameters);
                            newImage.Dispose();
                            finalFI = new FileInfo(newFileName);
                        }
                    }
                    fullsizeImage.Dispose();
                }
            }
            return finalFI;
        }

        private static Bitmap ForceResizeImage(Image image, int width, int height)
        {

            var newSize = new Size(width, height);

            var finalSize = new Size();
            var origRatio = GetRatio(image.Size);
            var newRatio = GetRatio(newSize);

            if (origRatio < newRatio)
            {

                //width is our base
                finalSize.Width = newSize.Width;
                finalSize.Height = (int)(finalSize.Width / origRatio);

            }
            else if (origRatio > newRatio)
            {
                //height is our base
                finalSize.Height = newSize.Height;
                finalSize.Width = (int)(finalSize.Height * origRatio);

            }
            else
            {
                //mathching!
                finalSize = newSize;
            }

            //where do we start?
            var x = width-finalSize.Width ;
            var y = height-finalSize.Height ;

            //a holder for the result
            var result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (var graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, x, y, finalSize.Width, finalSize.Height);
            }

            //return the resulting bitmap
            return result;
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <param name="force"></param>
        /// <returns>The resized image.</returns>
        public static Bitmap ActualResizeImage(Image image, int width, int height)
        {
            //a holder for the result
            var result = new Bitmap(width, height);

            //use a graphics object to draw the resized image into the bitmap
            using (var graphics = Graphics.FromImage(result))
            {
                //set the resize quality modes to high quality
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                //draw the image into the target bitmap
                graphics.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            //return the resulting bitmap
            return result;
        }




        private static float GetRatio(Size size)
        {
            return (float)size.Width / size.Height;
        }

        public static Size NewSize(Size origSize, Size newSize)
        {
            var finalSize = new Size();
            var origRatio = GetRatio(origSize);
            var newRatio = GetRatio(newSize);

            if (origRatio > newRatio)
            {

                //width is our base
                finalSize.Width = newSize.Width;
                finalSize.Height = (int)(finalSize.Width / origRatio);

            }
            else if (origRatio < newRatio)
            {
                //height is our base
                finalSize.Height = newSize.Height;
                finalSize.Width = (int)(finalSize.Height * origRatio);

            }
            else
            {
                //mathching!
                finalSize = newSize;
            }

            return finalSize;

        }


        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }




    }

    public class QueryHelper
    {
        public static string BuildQueryParam(Uri theUri, string key, string value)
        {
            var qVal = theUri.Query;
            if (theUri.Query.Length > 0)
            {
                qVal = theUri.Query.Substring(1, theUri.Query.Length - 1);
            }
            var vals = QueryHelper.SplitParams(qVal);


            var val2 = new Dictionary<string, string>();

            foreach (var val in vals)
            {
                if (val.Key == key)
                {
                    continue;
                }

                //handle clearing 'all'
                if (val.Key.ToLower() == "all" && key.ToLower() != "all")
                {
                    continue;
                }

                val2.Add(val.Key, val.Value);
            }

            if (!String.IsNullOrEmpty(value))
            {
                val2[key] = value;
            }
            var sb = new StringBuilder();
            sb.Append("?");

            foreach (var obKey in val2.Keys)
            {
                if (sb.Length > 1)
                {
                    sb.Append("&");
                }

                var queryVal = HttpUtility.UrlDecode(val2[obKey]);

                sb.Append(String.Format("{0}={1}", obKey, HttpUtility.UrlEncode(queryVal)));
            }

            return sb.ToString();
        }

        public static Dictionary<string, string> SplitParams(string uri)
        {
            var parts = uri.Split(Char.Parse("&"));

            var val = new Dictionary<string, string>();

            foreach (var part in parts)
            {
                var item = part.Split(Char.Parse("="));
                if (item.Length == 2)
                {
                    val[item[0].ToLower()] = item[1];
                }
            }

            return val;
        }
    }

    public class ImageDownloader
    {
        public static FileInfo DownloadAndSave(string imageUrl, string destinationFile)
        {
            try
            {
                var client = new WebClient();
                using (var stream = client.OpenRead(imageUrl))
                {
                    if (stream != null)
                    {

                        using (var bitmap = new Bitmap(stream))
                        {
                            stream.Flush();
                            stream.Close();
                            stream.Dispose();
                            bitmap.Save(destinationFile);
                            bitmap.Dispose();
                        }

                        var fi = new FileInfo(destinationFile);

                        return fi;
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

    }

}