using System;
using System.Collections.Generic;
using System.Text;

namespace My9GAG.NineGagApiClient.Utils
{
    public class NineGagOptions
    {
        public string ApiUrl { get; set; }
        public string CommentCdnUrl { get; set; }
        public string CommentUrl { get; set; }
        public string NotifyUrl { get; set; }
        public string AdUrl { get; set; }
        public string AdminUrl { get; set; }

        public static NineGagOptions CreateDefaultOptions()
        {
            return new NineGagOptions
            {
                ApiUrl = "https://api.9gag.com",
                CommentCdnUrl = "https://comment-cdn.9gag.com",
                CommentUrl = "https://comment.9gag.com",
                NotifyUrl = "https://notify.9gag.com",
                AdUrl = "https://ad.9gag.com",
                AdminUrl = "https://admin.9gag.com"
            };
        }
    }
}
