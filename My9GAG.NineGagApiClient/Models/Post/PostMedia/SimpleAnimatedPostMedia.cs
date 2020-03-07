
namespace My9GAG.Models.Post.Media
{
    public class SimpleAnimatedPostMedia : ISimplePostMedia
    {
        #region Properties

        public PostType Type
        {
            get { return PostType.Animated; }
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
