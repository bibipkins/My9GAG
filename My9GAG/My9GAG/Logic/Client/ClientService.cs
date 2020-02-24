using My9GAG.Logic.Logger;
using My9GAG.Logic.Request;
using My9GAG.Logic.SecureStorage;
using My9GAG.Models.Authentication;
using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.Models.Post.Media;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace My9GAG.Logic.Client
{
    public class ClientService : IClientService
    {
        #region Constructors

        public ClientService(ILogger logger, ISecureStorage secureStorage)
        {
            _logger = logger;
            _secureStorage = secureStorage;

            Posts = new List<Post>();
            Comments = new List<Comment>();
            AuthenticationInfo = new AuthenticationInfo();
        }

        #endregion

        #region Properties

        public List<Post> Posts
        {
            get;
            private set;
        }
        public List<Comment> Comments
        {
            get;
            private set;
        }
        public AuthenticationInfo AuthenticationInfo
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public async Task<RequestStatus> LoginWithCredentialsAsync(string userName, string password)
        {
            var args = new Dictionary<string, string>()
            {
                { "loginMethod", "9gag" },
                { "loginName", userName },
                { "password", RequestUtils.GetMd5(password) },
                { "language", "en_US" },
                { "pushToken", AuthenticationInfo.Token }
            };

            var requestStatus = await LoginAsync(args, AuthenticationType.Credentials);

            if (requestStatus.IsSuccessful)
            {
                AuthenticationInfo.UserLogin = userName;
                AuthenticationInfo.UserPassword = password;
            }

            return requestStatus;
        }
        public async Task<RequestStatus> LoginWithGoogleAsync(string token)
        {
            var args = new Dictionary<string, string>()
            {
                { "userAccessToken", token },
                { "loginMethod", "google-plus" },
                { "language", "en_US" },
                { "pushToken", AuthenticationInfo.Token }
            };

            var requestStatus = await LoginAsync(args, AuthenticationType.Google);
            return requestStatus;
        }
        public async Task<RequestStatus> LoginWithFacebookAsync(string token)
        {
            var args = new Dictionary<string, string>()
            {
                { "loginMethod", "facebook" },
                { "userAccessToken", token },                
                { "language", "en_US" },
                { "pushToken", AuthenticationInfo.Token }
            };

            var loginStatus = await LoginAsync(args, AuthenticationType.Facebook);
            return loginStatus;
        }
        public async Task Logout()
        {
            AuthenticationInfo.ClearToken();
            await SaveAuthenticationInfoAsync();
        }

        public async Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThan = "")
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

            if (!string.IsNullOrEmpty(olderThan))
            {
                args["olderThan"] = olderThan;
            }
            
            var request = FormRequest(RequestUtils.API, RequestUtils.POSTS_PATH, args);
            var requestStatus = await ExecuteRequestAsync(request, responseText =>
            {
                Posts = new List<Post>();

                var jsonData = JObject.Parse(responseText);
                var rawPosts = jsonData["data"]["posts"];

                foreach (var item in rawPosts)
                {
                    Post post = item.ToObject<Post>();
                    post.PostMedia = PostMediaFactory.CreatePostMedia(post.Type, item);
                    post.PostMedia.GenerateView();
                    Posts.Add(post);
                }
            });

            return requestStatus;
        }
        public async Task<RequestStatus> GetCommentsAsync(string postUrl, int count)
        {
            string path = 
                "v1/topComments.json?" +
                "appId=a_dd8f2b7d304a10edaf6f29517ea0ca4100a43d1b" +
                "&urls=" + postUrl +
                "&commentL1=" + count +
                "&pretty=0";

            var request = FormRequest(RequestUtils.COMMENT_CDN, path, new Dictionary<string, string>());
            var requestStatus = await ExecuteRequestAsync(request, responseText =>
            {
                Comments = new List<Comment>();
                var jsonData = JObject.Parse(responseText);
                var comments = jsonData.SelectToken("payload.data.[0].comments");

                foreach (var item in comments)
                {
                    Comment comment = item.ToObject<Comment>();
                    comment.MediaUrl = GetUrlFromJsonComment(item);
                    Comments.Add(comment);
                }

                Comments = Comments.OrderByDescending(c => c.LikesCount).ToList();
            });

            return requestStatus;
        }
        public async Task<RequestStatus> GetGroupsAsync()
        {
            var request = FormRequest(RequestUtils.API, RequestUtils.GROUPS_PATH);
            var requestStatus = await ExecuteRequestAsync(request, responseText =>
            {

            });

            return requestStatus;
        }

        public async Task LoadAuthenticationInfoAsync()
        {
            var result = await _secureStorage.GetAsync(AUTHENTICATION_INFO_KEY);

            if (result.IsSuccessful)
            {
                var jsonData = JObject.Parse(result.Value);
                AuthenticationInfo = jsonData.ToObject<AuthenticationInfo>();
            }
        }
        public async Task SaveAuthenticationInfoAsync()
        {
            string authenticationInfoJson = JObject.FromObject(AuthenticationInfo).ToString();
            await _secureStorage.SetAsync(AUTHENTICATION_INFO_KEY, authenticationInfoJson);
        }

        public void SaveState(IDictionary<string, object> dictionary)
        {

        }
        public void RestoreState(IDictionary<string, object> dictionary)
        {

        }

        #endregion

        #region Implementation

        private T GetDictionaryEntry<T>(IDictionary<string, object> dictionary, string key, T defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return (T)dictionary[key];
            }

            return defaultValue;
        }
        private async Task<RequestStatus> LoginAsync(Dictionary<string, string> args, AuthenticationType authenticationType)
        {
            var request = FormRequest(RequestUtils.API, RequestUtils.LOGIN_PATH, args);
            var requestStatus = await ExecuteRequestAsync(request, responseText =>
            {
                var jsonData = JObject.Parse(responseText);
                var authData = jsonData["data"];

                AuthenticationInfo.LastAuthenticationType = authenticationType;
                AuthenticationInfo.Token = authData["userToken"].ToString();

                long.TryParse(authData["tokenExpiry"].ToString(), out long seconds);
                AuthenticationInfo.TokenWillExpireAt = DateTimeOffset.FromUnixTimeSeconds(seconds).UtcDateTime;

                string readStateParams = authData["noti"]["readStateParams"].ToString();
                _generatedAppId = RequestUtils.ExtractValueFromUrl(readStateParams, "appId");
            });

            if (requestStatus.IsSuccessful)
            {
                await SaveAuthenticationInfoAsync();
            }

            return requestStatus;
        }
        private async Task<RequestStatus> ExecuteRequestAsync(HttpWebRequest request, Action<string> onSuccess)
        {
            var requestStatus = new RequestStatus();

            try
            {
                using (var response = (HttpWebResponse)(await request.GetResponseAsync()))
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string responseText = await reader.ReadToEndAsync();
                    requestStatus = ValidateResponse(responseText);

                    if (requestStatus.IsSuccessful)
                    {
                        onSuccess(responseText);
                    }
                }
            }
            catch (Exception e)
            {
                requestStatus.IsSuccessful = false;
                requestStatus.Message = e.Message;
                _logger.Log(e.Message, e.StackTrace);
            }

            return requestStatus;
        }
        private HttpWebRequest FormRequest(string api, string path)
        {
            var args = new Dictionary<string, string>();
            return FormRequest(api, path, args);
        }
        private HttpWebRequest FormRequest(string api, string path, Dictionary<string, string> args)
        {
            var headers = new Dictionary<string, string>()
            {
                { "9GAG-9GAG_TOKEN", AuthenticationInfo.Token },
                { "9GAG-TIMESTAMP", AuthenticationInfo.Timestamp },
                { "9GAG-APP_ID", AuthenticationInfo.AppId },
                { "X-Package-ID", AuthenticationInfo.AppId },
                { "9GAG-DEVICE_UUID", AuthenticationInfo.DeviceUuid },
                { "X-Device-UUID", AuthenticationInfo.DeviceUuid },
                { "9GAG-DEVICE_TYPE", "android" },
                { "9GAG-BUCKET_NAME", "MAIN_RELEASE" },
                { "9GAG-REQUEST-SIGNATURE", AuthenticationInfo.Signature }
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
        private RequestStatus ValidateResponse(string response)
        {
            var requestStatus = new RequestStatus();

            try
            {
                var jsonData = JObject.Parse(response);

                if (jsonData.ContainsKey("meta"))
                {
                    if (jsonData["meta"]["status"].ToString() == "Success")
                    {
                        requestStatus.IsSuccessful = true;
                        requestStatus.Message = "";
                    }
                    else
                    {
                        requestStatus.IsSuccessful = false;
                        requestStatus.Message = jsonData["meta"]["errorMessage"].ToString();
                    }
                }
                else if (jsonData.ContainsKey("status"))
                {
                    if (jsonData["status"].ToString() == "ERROR")
                    {
                        requestStatus.IsSuccessful = false;
                        requestStatus.Message = jsonData["error"].ToString();
                    }
                    else if (jsonData["status"].ToString() == "OK")
                    {
                        requestStatus.IsSuccessful = true;
                        requestStatus.Message = "";
                    }
                }
            }
            catch (Exception e)
            {
                requestStatus.IsSuccessful = false;
                requestStatus.Message = e.Message;
                _logger.Log(e.Message, e.StackTrace);
            }

            return requestStatus;
        }
        private string GetUrlFromJsonComment(JToken token)
        {
            var urlToken =
                token.SelectToken("media.[0].imageMetaByType.animated.url") ??
                token.SelectToken("media.[0].imageMetaByType.image.url") ??
                string.Empty;

            return urlToken.ToString();
        }
        
        #endregion

        #region Fields

        private readonly ILogger _logger;
        private readonly ISecureStorage _secureStorage;

        private string _generatedAppId = "";

        #endregion

        #region Constants

        private const string AUTHENTICATION_INFO_KEY = "AUTHENTICATION_INFO_KEY";

        #endregion
    }
}
