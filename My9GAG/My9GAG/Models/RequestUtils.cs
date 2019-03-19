using System;
using System.Security.Cryptography;
using System.Text;

namespace My9GAG.Models
{
    public static class RequestUtils
    {
        public static string GetTimestamp()
        {
            return ((long)(DateTime.Now.Subtract(DateTime.MinValue.AddYears(1969)).TotalMilliseconds)).ToString();
        }
        public static string GetUUID()
        {
            return (Guid.NewGuid().ToString()).Replace("-", String.Empty).ToLower();
        }
        public static string GetSHA1(string data)
        {
            using (var sha1 = new SHA1Managed())
                return (BitConverter.ToString(sha1.ComputeHash(Encoding.UTF8.GetBytes(data)))).Replace("-", String.Empty).ToLower();
        }
        public static string GetMD5(string data)
        {
            using (var md5 = new MD5CryptoServiceProvider())
                return (BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(data)))).Replace("-", String.Empty).ToLower();
        }
        public static string GetSignature(string timestamp, string appID, string deviceUUID)
        {
            string data = String.Format("*{0}_._{1}._.{2}9GAG", timestamp, appID, deviceUUID);
            return GetSHA1(data);
        }

        public static string API =         "http://api.9gag.com";
        public static string COMMENT_CDN = "http://comment-cdn.9gag.com";
        public static string COMMENT =     "http://comment.9gag.com";
        public static string NOTIFY =      "http://notify.9gag.com";
        public static string AD =          "http://ad.9gag.com";
        public static string ADMIN =       "http://admin.9gag.com";
        public static string APP_ID =      "com.ninegag.android.app";
    }
}
