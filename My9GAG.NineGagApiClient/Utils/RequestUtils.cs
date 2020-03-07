using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace My9GAG.NineGagApiClient.Utils
{
    public static class RequestUtils
    {
        #region Methods

        public static string GetTimestamp()
        {
            return ((long)DateTime.UtcNow.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds).ToString();
        }
        public static string GetUuid()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();
        }
        public static string GetSha1(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                using (var sha1 = new SHA1Managed())
                    return BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", string.Empty).ToLower();
            }
            else
            {
                return "";
            }
        }
        public static string GetMd5(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                using (var md5 = new MD5CryptoServiceProvider())
                    return BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(data))).Replace("-", string.Empty).ToLower();
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

        public static string LOGIN_PATH = "v2/user-token";
        public static string POSTS_PATH = "v2/post-list";
        public static string GROUPS_PATH = "v2/group-list";

        #endregion
    }
}
