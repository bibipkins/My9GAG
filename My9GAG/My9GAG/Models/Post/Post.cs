using My9GAG.Models.Post.Media;
using Newtonsoft.Json;
using System.Net;

namespace My9GAG.Models.Post
{
    public class Post
    {
        #region Properties

        public string Id
        {
            get;
            set;
        }
        public string Title
        {
            get { return _title; }
            set { _title = WebUtility.HtmlDecode(value); }
        }
        public string Url
        {
            get;
            set;
        }
        public int UpvoteCount
        {
            get;
            set;
        }
        public int DownvoteCount
        {
            get;
            set;
        }
        public int CommentCount
        {
            get;
            set;
        }        
        public PostType Type
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "postSection")]
        public PostSection Section
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "nsfw")]
        public bool IsNsfw
        {
            get;
            set;
        }
        public IPostMedia PostMedia
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Id}, {Title}, URL: {Url}, MediaURL: {PostMedia?.Url}, " +
                $"CommentsCount: {CommentCount}, UpvoteCount: {UpvoteCount}, " +
                $"DownvoteCount: {DownvoteCount}, Type: {Type.ToString()}, NSFW: {IsNsfw}, " +
                $"Section: {Section?.Name}";
        }

        #endregion

        #region Fields

        private string _title;

        #endregion
    }
}
