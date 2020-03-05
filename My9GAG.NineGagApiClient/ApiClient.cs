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
using System.Net.Http;
using System.Threading.Tasks;

namespace My9GAG.NineGagApiClient
{
    public class ApiClient : IApiClient, IDisposable
    {
        private readonly NineGagOptions _nineGagOptions;
        private readonly HttpClient _httpClient;
        private readonly bool _disposeHttpClient;

        public AuthenticationInfo AuthenticationInfo { get; protected set; }

        public ApiClient() : this(new HttpClient(), nineGagOptionsBuilder: null)
        {
            _disposeHttpClient = true;
        }

        public ApiClient(HttpClient httpClient, Action<NineGagOptions> nineGagOptionsBuilder)
        {
            _disposeHttpClient = false;
            _nineGagOptions = NineGagOptions.CreateDefaultOptions();
            nineGagOptionsBuilder?.Invoke(_nineGagOptions);

            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            AuthenticationInfo = CreateAuthenticationInfo();
        }

        #region Api Functionality

        public virtual async Task<IList<SimplePost>> GetPostsAsync(PostCategory postCategory, int count, string olderThanPostId = "")
        {
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
            var request = FormRequest(_nineGagOptions.ApiUrl, RequestUtils.POSTS_PATH, args);
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

        public virtual async Task<IList<Comment>> GetCommentsAsync(string postUrl, int count)
        {
            string path =
                "v1/topComments.json?" +
                "appId=a_dd8f2b7d304a10edaf6f29517ea0ca4100a43d1b" +
                "&urls=" + postUrl +
                "&commentL1=" + count +
                "&pretty=0";

            var comments = new List<Comment>();
            var request = FormRequest(_nineGagOptions.CommentCdnUrl, path, new Dictionary<string, string>());
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
        protected virtual string GetUrlFromJsonComment(JToken token)
        {
            var urlToken =
                token.SelectToken("media.[0].imageMetaByType.animated.url") ??
                token.SelectToken("media.[0].imageMetaByType.image.url") ??
                string.Empty;

            return urlToken.ToString();
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

        public virtual async Task LoginWithGoogleAsync(string token)
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

        public virtual async Task LoginWithFacebookAsync(string token)
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
        public virtual void Logout()
        {
            AuthenticationInfo.ClearToken();
        }

        protected async Task LoginAsync(Dictionary<string, string> args, AuthenticationType authenticationType)
        {
            var request = FormRequest(_nineGagOptions.ApiUrl, RequestUtils.LOGIN_PATH, args);
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
        protected virtual async Task ExecuteRequestAsync(HttpRequestMessage request, Action<string> onSuccess = null)
        {
            using (var response = await _httpClient.SendAsync(request))
            {
                var responseText = await response.Content.ReadAsStringAsync();
                ValidateResponse(responseText);

                onSuccess?.Invoke(responseText);
            }
        }
        protected virtual HttpRequestMessage FormRequest(string api, string path, Dictionary<string, string> args)
        {
            var timestamp = RequestUtils.GetTimestamp();

            var headers = new Dictionary<string, string>()
            {
                { "User-Agent", ".NET" },
                //{ "Content-Type", "application/json" },
                { "Accept", " */*" },
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
                argsStrings.Add(string.Format("{0}/{1}", entry.Key, entry.Value));
            }

            var urlItems = new List<string>()
            {
                api,
                path,
                string.Join("/", argsStrings)
            };

            var url = string.Join("/", urlItems);
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            foreach (var entry in headers)
            {
                request.Headers.Add(entry.Key, entry.Value);
            }

            return request;
        }

        protected virtual void ValidateResponse(string response)
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

        public virtual void Dispose()
        {
            if (_disposeHttpClient)
            {
                _httpClient.Dispose();
            }
        }
        #endregion
    }
}
