using Newtonsoft.Json;
using System.Net;

namespace My9GAG.Models.Comment
{
    public class Comment
    {
        #region Properties

        [JsonProperty(PropertyName = "commentId")]
        public string Id
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "parent")]
        public string ParentId
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "likeCount")]
        public int LikesCount
        {
            get;
            set;
        }
        [JsonProperty(PropertyName = "dislikeCount")]
        public int DislikesCount
        {
            get;
            set;
        }
        public long Timestamp
        {
            get;
            set;
        }
        public User.User User
        {
            get;
            set;
        }
        public CommentType Type
        {
            get;
            set;
        }

        [JsonProperty(PropertyName = "mediaText")]
        public string Text
        {
            get { return _text; }
            set { _text = WebUtility.HtmlDecode(value); }
        }
        public bool IsTextVisible
        {
            get { return !string.IsNullOrEmpty(Text); }
        }

        public string MediaUrl
        {
            get;
            set;
        }
        public bool IsMediaPresent
        {
            get { return !string.IsNullOrEmpty(MediaUrl); }
        }

        #endregion

        #region Fields

        private string _text;

        #endregion
    }
}
