using Newtonsoft.Json;
using System.Net;

namespace My9GAG.Models
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
        public int LikeCount
        {
            get;
            set;
        }
        public int DislikeCount
        {
            get;
            set;
        }
        public long Timestamp
        {
            get;
            set;
        }
        public User User
        {
            get;
            set;
        }
        public CommentType Type
        {
            get;
            set;
        }
        public string Text
        {
            get { return _text; }
            set { _text = WebUtility.HtmlDecode(value); }
        }

        #endregion

        #region Fields

        private string _text;

        #endregion
    }
}
