
namespace My9GAG.Models.Post.Media
{
    public class SimpleImagePostMedia : ISimplePostMedia
    {
        #region Properties

        public PostType Type
        {
            get { return PostType.Photo; }
        }
        public string Url
        {
            get;
            set;
        }
        public double Width
        {
            get;
            set;
        }
        public double Height 
        {
            get;
            set;
        }

        #endregion

    }
}
