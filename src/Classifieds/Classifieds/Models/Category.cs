namespace Classifieds.Models
{
    public class Category : GuidObject
    {
        public string Name { get; set; }
        public string Parent { get; set; }

        public override string Code
        {
            get { return Name; }
        }


    }
}