using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace My9GAG.Logic.Request
{
    public static class RequestUtils
    {
        #region Methods

        public static string GetTimestamp()
        {
            return ((long)(DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds)).ToString();
        }
        public static string GetUuid()
        {
            return (Guid.NewGuid().ToString()).Replace("-", String.Empty).ToLower();
        }
        public static string GetSha1(string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                using (var sha1 = new SHA1Managed())
                    return (BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(data)))).Replace("-", String.Empty).ToLower();
            }
            else
            {
                return "";
            }
        }
        public static string GetMd5(string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                using (var md5 = new MD5CryptoServiceProvider())
                    return (BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(data)))).Replace("-", String.Empty).ToLower();
            }
            else
            {
                return "";
            }
        }
        public static string GetSignature(string timestamp, string appId, string deviceUuid)
        {
            string data = $"*{timestamp}_._{appId}._.{deviceUuid}9GAG";
            return GetSha1(data);
        }
        public static string ExtractValueFromUrl(string url, string key)
        {
            string value = string.Empty;
            char attributeDivider = '&';
            char keyValueDivider = '=';
            string fullKey = key + keyValueDivider;

            if (url.Contains(fullKey))
            {
                var attributes = url.Split(attributeDivider);
                var keyValuePair = attributes.FirstOrDefault(s => s.Contains(fullKey)).Split(keyValueDivider);

                if (keyValuePair.Length > 1)
                {
                    value = keyValuePair[1];
                }
            }

            return value;
        }

        #endregion

        #region Constants

        public static string API =         "http://api.9gag.com";
        public static string COMMENT_CDN = "http://comment-cdn.9gag.com";
        public static string COMMENT =     "http://comment.9gag.com";
        public static string NOTIFY =      "http://notify.9gag.com";
        public static string AD =          "http://ad.9gag.com";
        public static string ADMIN =       "http://admin.9gag.com";

        public static string LOGIN_PATH =  "v2/user-token";
        public static string POSTS_PATH =  "v2/post-list";
        public static string GROUPS_PATH = "v2/group-list";

        #endregion
    }
}
