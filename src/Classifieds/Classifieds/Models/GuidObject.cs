namespace Classifieds.Models
{
    public abstract class GuidObject
    {
        public abstract string Code { get; }

        public string Guid
        {
            get { return string.Format("{0}/{1}", this.GetType().Name, this.Code); }
        }
    }
}