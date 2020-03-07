using My9GAG.Models.Post.Media;
using Newtonsoft.Json;
using System.Net;

namespace My9GAG.Models.Post
{
    //We call these simple so we don't conflict with the class names in the My9GAG project
    //The reason we use the same namespace as the My9GAG project is for backward compatibility
    //and because I can't refactor the UWP, IOS and Android projects (missing plugins)
    public class SimplePost
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
        public int CommentsCount
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
        public ISimplePostMedia SimplePostMedia
        {
            get;
            set;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Id}, {Title}, URL: {Url}, MediaURL: {SimplePostMedia?.Url}, " +
                $"CommentsCount: {CommentsCount}, UpvoteCount: {UpvoteCount}, " +
                $"DownvoteCount: {DownvoteCount}, Type: {Type.ToString()}, NSFW: {IsNsfw}, " +
                $"Section: {Section?.Name}";
        }

        #endregion

        #region Fields

        private string _title;

        #endregion
    }
}
