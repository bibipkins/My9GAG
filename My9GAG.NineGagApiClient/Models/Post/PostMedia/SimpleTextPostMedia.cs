namespace My9GAG.Models.Post.Media
{
    public class SimpleTextPostMedia : ISimplePostMedia
    {
        #region Properties
        public PostType Type
        {
            get { return PostType.Other; }
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
        public string Text
        {
            get;
            set;
        }
        
        #endregion

    }
}
