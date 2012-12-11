using System.Collections.Generic;
using System.Configuration;

namespace Classifieds.services
{
    public class ImageSizeSection : ConfigurationSection
    {
        public static ImageSizeSection GetConfig()
        {
            return ConfigurationManager.GetSection("imageConfig/imageSection") as ImageSizeSection;
        }

        [ConfigurationProperty("ImageSizes")]
        public ImageSizeCollection ImageSizes
        {
            get { return this["ImageSizes"] as ImageSizeCollection; }
        }

    }


    public class ImageSize : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get { return this["name"] as string; } }

        [ConfigurationProperty("width", IsRequired = true)]
        public int Width { get { return int.Parse(this["width"].ToString()); } }

        [ConfigurationProperty("height", IsRequired = false)]
        public int Height { get { return int.Parse(this["height"].ToString()); } }

    }


    public class ImageSizeCollection : ConfigurationElementCollection, IEnumerable<ImageSize>
    {
        public ImageSize this[int index]
        {
            get { return base.BaseGet(index) as ImageSize; }
            set
            {
                if (base.BaseGet(index) != null)
                    base.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }

        public List<ImageSize> Servers
        {
            get
            {
                var servers = new List<ImageSize>();
                var e = base.GetEnumerator();
                while (e.MoveNext())
                {
                    servers.Add((ImageSize)e.Current);
                }

                return servers;
            }
        }

        // to get by key image instead of index
        public new ImageSize this[string key]
        {
            get { return base.BaseGet(key) as ImageSize; }
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new ImageSize();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ImageSize)element).Name;
        }


        public new IEnumerator<ImageSize> GetEnumerator()
        {
            return Servers.GetEnumerator();
        }
    }
}
