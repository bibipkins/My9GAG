using Newtonsoft.Json;
using System.Net;

namespace My9GAG.Models
{
    public class Post
    {
        public string Id { get; set; }
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = WebUtility.HtmlDecode(value);
            }
        }
        public string Url { get; set; }
        public string MediaUrl { get; set; }
        public int UpvoteCount { get; set; }
        public int DownvoteCount { get; set; }
        public int CommentCount { get; set; }        
        public PostType Type { get; set; }
        [JsonProperty(PropertyName = "postSection")]
        public PostSection Section { get; set; }
        [JsonProperty(PropertyName = "nsfw")]
        public bool IsNsfw { get; set; }

        public override string ToString()
        {
            return $"{Id}, {Title}, URL: {Url}, MediaURL: {MediaUrl}, " +
                $"CommentsCount: {CommentCount}, UpvoteCount: {UpvoteCount}, " +
                $"DownvoteCount: {DownvoteCount}, Type: {Type.ToString()}, NSFW: {IsNsfw}, " +
                $"Section: {Section?.Name}";
        }

        #region Fields

        private string title;

        #endregion
    }
}
