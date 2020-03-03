using My9GAG.Models.Authentication;
using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.Models.Post.Media;
using My9GAG.NineGagApiClient.Models.Authentication;
using My9GAG.NineGagApiClient.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace My9GAG.NineGagApiClient
{
    public class ApiClient : IApiClient
    {
        public AuthenticationInfo AuthenticationInfo { get; protected set; }

        public ApiClient()
        {
            AuthenticationInfo = CreateAuthenticationInfo();
        }

        #region Api Functionality

        public async Task<IEnumerable<SimplePost>> GetPostsAsync(PostCategory postCategory, int count, string olderThanPostId = "")
        {
            await GetGroupsAsync();

            string type = postCategory.ToString().ToLower();
            var args = new Dictionary<string, string>()
            {
                { "group", "1" },
                { "type", type },
                { "itemCount", count.ToString() },
                { "entryTypes", "animated,photo,video" },
                { "offset", "10" }
            };

            if (!string.IsNullOrEmpty(olderThanPostId))
            {
                args["olderThan"] = olderThanPostId;
            }

            var posts = new List<SimplePost>();
            var request = FormRequest(RequestUtils.API, RequestUtils.POSTS_PATH, args);
            await ExecuteRequestAsync(request, responseText =>
           {

               var jsonData = JObject.Parse(responseText);
               var rawPosts = jsonData["data"]["posts"];

               foreach (var item in rawPosts)
               {
                   var post = CreatePost(item);
                   posts.Add(post);
               }
           });

            return posts;
        }

        protected virtual SimplePost CreatePost(JToken item)
        {
            var post = item.ToObject<SimplePost>();
            post.SimplePostMedia = SimplePostMediaFactory.CreatePostMedia(post.Type, item);
            return post;
        }

        public async Task<IEnumerable<Comment>> GetCommentsAsync(string postUrl, int count)
        {
            string path =
                "v1/topComments.json?" +
                "appId=a_dd8f2b7d304a10edaf6f29517ea0ca4100a43d1b" +
                "&urls=" + postUrl +
                "&commentL1=" + count +
                "&pretty=0";

            var comments = new List<Comment>();
            var request = FormRequest(RequestUtils.COMMENT_CDN, path, new Dictionary<string, string>());
            await ExecuteRequestAsync(request, responseText =>
            {
                var jsonData = JObject.Parse(responseText);
                var postComments = jsonData.SelectToken("payload.data.[0].comments");

                foreach (var item in postComments)
                {
                    Comment comment = item.ToObject<Comment>();
                    comment.MediaUrl = GetUrlFromJsonComment(item);
                    comments.Add(comment);
                }

                comments = comments.OrderByDescending(c => c.LikesCount).ToList();
            });

            return comments;
        }
        private string GetUrlFromJsonComment(JToken token)
        {
            var urlToken =
                token.SelectToken("media.[0].imageMetaByType.animated.url") ??
                token.SelectToken("media.[0].imageMetaByType.image.url") ??
                string.Empty;

            return urlToken.ToString();
        }

        public async Task GetGroupsAsync()
        {//TODO figure out why this is here
            var request = FormRequest(RequestUtils.API, RequestUtils.GROUPS_PATH);
            await ExecuteRequestAsync(request);
        }
        #endregion

        #region Auth methods
        protected virtual AuthenticationInfo CreateAuthenticationInfo()
        {
            return new AuthenticationInfo();
        }

        public virtual async Task LoginWithCredentialsAsync(string userName, string password)
        {
            var args = new Dictionary<string, string>()
            {
                { "loginMethod", "9gag" },
                { "loginName", userName },
                { "password", RequestUtils.GetMd5(password) },
                { "language", "en_US" },
                { "pushToken", AuthenticationInfo.Token }
            };

            await LoginAsync(args, AuthenticationType.Credentials);
            AuthenticationInfo.UserLogin = userName;
            AuthenticationInfo.UserPassword = password;
        }

        public async Task LoginWithGoogleAsync(string token)
        {
            var args = new Dictionary<string, string>()
                {
                    { "userAccessToken", token },
                    { "loginMethod", "google-plus" },
                    { "language", "en_US" },
                    { "pushToken", AuthenticationInfo.Token }
                };

            await LoginAsync(args, AuthenticationType.Google);
        }

        public async Task LoginWithFacebookAsync(string token)
        {
            var args = new Dictionary<string, string>()
            {
                { "loginMethod", "facebook" },
                { "userAccessToken", token },
                { "language", "en_US" },
                { "pushToken", AuthenticationInfo.Token }
            };

            await LoginAsync(args, AuthenticationType.Facebook);
        }
        public void Logout()
        {
            AuthenticationInfo.ClearToken();
        }

        protected async Task LoginAsync(Dictionary<string, string> args, AuthenticationType authenticationType)
        {
            var request = FormRequest(RequestUtils.API, RequestUtils.LOGIN_PATH, args);
            await ExecuteRequestAsync(request, responseText =>
            {
                var jsonData = JObject.Parse(responseText);
                var authData = jsonData["data"];

                AuthenticationInfo.Token = authData["userToken"].ToString();
                AuthenticationInfo.LastAuthenticationType = authenticationType;

                long.TryParse(authData["tokenExpiry"].ToString(), out long seconds);
                AuthenticationInfo.TokenWillExpireAt = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;

                string readStateParams = authData["noti"]["readStateParams"].ToString();
            });
        }
        #endregion

        #region Helpers
        private async Task ExecuteRequestAsync(HttpWebRequest request, Action<string> onSuccess = null)
        {
            using (var response = (HttpWebResponse)(await request.GetResponseAsync()))
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            {
                string responseText = await reader.ReadToEndAsync();
                ValidateResponse(responseText);

                onSuccess?.Invoke(responseText);
            }
        }
        private HttpWebRequest FormRequest(string api, string path)
        {
            var args = new Dictionary<string, string>();
            return FormRequest(api, path, args);
        }
        private HttpWebRequest FormRequest(string api, string path, Dictionary<string, string> args)
        {
            var timestamp = RequestUtils.GetTimestamp();

            var headers = new Dictionary<string, string>()
            {
                { "9GAG-9GAG_TOKEN", AuthenticationInfo.Token },
                { "9GAG-TIMESTAMP", timestamp },
                { "9GAG-APP_ID", AuthenticationInfo.AppId },
                { "X-Package-ID", AuthenticationInfo.AppId },
                { "9GAG-DEVICE_UUID", AuthenticationInfo.DeviceUuid },
                { "X-Device-UUID", AuthenticationInfo.DeviceUuid },
                { "9GAG-DEVICE_TYPE", "android" },
                { "9GAG-BUCKET_NAME", "MAIN_RELEASE" },
                { "9GAG-REQUEST-SIGNATURE", RequestUtils.GetSignature(timestamp, AuthenticationInfo.AppId, AuthenticationInfo.DeviceUuid) }
            };

            var argsStrings = new List<string>();

            foreach (var entry in args)
            {
                argsStrings.Add(String.Format("{0}/{1}", entry.Key, entry.Value));
            }

            var urlItems = new List<string>()
            {
                api,
                path,
                String.Join("/", argsStrings)
            };

            string url = String.Join("/", urlItems);
            var request = (HttpWebRequest)WebRequest.Create(url);
            var headerCollection = new WebHeaderCollection();

            foreach (var entry in headers)
            {
                headerCollection.Add(entry.Key, entry.Value);
            }

            request.Headers = headerCollection;
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = ".NET";
            request.ContentType = "application/json";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Accept = "*/*";

            return request;
        }

        private void ValidateResponse(string response)
        {
            var jsonData = JObject.Parse(response);

            if (jsonData.ContainsKey("meta"))
            {
                if (jsonData["meta"]["status"].ToString() != "Success")
                {
                    throw new InvalidOperationException($"Request failed: {jsonData["meta"]["errorMessage"].ToString()}");
                }
            }
            else if (jsonData.ContainsKey("status"))
            {
                if (jsonData["status"].ToString() == "ERROR")
                {
                    throw new InvalidOperationException($"Request failed: {jsonData["error"].ToString()}");

                }
            }
        }
        #endregion
    }
}
