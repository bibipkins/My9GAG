using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.Models.Post.Media;
using My9GAG.Models.Request;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace My9GAG.Logic
{
    public class ClientService : IClientService
    {
        #region Constructors

        public ClientService()
        {
            _timestamp  = RequestUtils.GetTimestamp();
            _appId      = RequestUtils.APP_ID;
            _token      = RequestUtils.GetSha1(_timestamp);
            _deviceUuid = RequestUtils.GetUuid();
            _signature  = RequestUtils.GetSignature(_timestamp, _appId, _deviceUuid);

            Posts = new List<Post>();
            Comments = new List<Comment>();
        }

        #endregion

        #region Properties

        public List<Post> Posts { get; private set; }
        public List<Comment> Comments { get; private set; }

        #endregion

        #region Methods

        public async Task<RequestStatus> LoginAsync(string userName, string password)
        {
            var args = new Dictionary<string, string>()
            {
                { "loginMethod", "9gag" },
                { "loginName", userName },
                { "password", RequestUtils.GetMd5(password) },
                { "language", "en_US" },
                { "pushToken", _token }
            };
            HttpWebRequest request = FormRequest(RequestUtils.API, RequestUtils.LOGIN_PATH, args);
            RequestStatus loginStatus = new RequestStatus();
            
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseText = await reader.ReadToEndAsync();
                    loginStatus = ValidateResponse(responseText);
                    
                    if (loginStatus.IsSuccessful)
                    {
                        var jsonData = JObject.Parse(responseText);

                        _token = jsonData["data"]["userToken"].ToString();
                        _userData = jsonData["data"].ToString();

                        string readStateParams = jsonData["data"]["noti"]["readStateParams"].ToString();

                        _generatedAppId = RequestUtils.ExtractValueFromUrl(readStateParams, "appId");

                        Debug.WriteLine("TOKEN: " + _token);
                        Debug.WriteLine("APP_ID: " + _generatedAppId);
                    }
                }
            }
            catch (Exception ex)
            {
                loginStatus.IsSuccessful = false;
                loginStatus.Message = ex.Message;
            }

            return loginStatus;
        }
        public async Task<RequestStatus> LoginWithGoogleAsync(string token)
        {
            var args = new Dictionary<string, string>()
            {
                { "userAccessToken", token },
                { "loginMethod", "google-plus" },
                { "language", "en_US" },
                { "pushToken", _token }
            };
            HttpWebRequest request = FormRequest(RequestUtils.API, RequestUtils.LOGIN_PATH, args);
            RequestStatus loginStatus = new RequestStatus();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseText = await reader.ReadToEndAsync();

                    loginStatus = ValidateResponse(responseText);

                    if (loginStatus.IsSuccessful)
                    {
                        var jsonData = JObject.Parse(responseText);

                        _token = jsonData["data"]["userToken"].ToString();
                        _userData = jsonData["data"].ToString();

                        string readStateParams = jsonData["data"]["noti"]["readStateParams"].ToString();

                        _generatedAppId = RequestUtils.ExtractValueFromUrl(readStateParams, "appId");

                        Debug.WriteLine("TOKEN: " + _token);
                        Debug.WriteLine("APP_ID: " + _generatedAppId);
                    }
                }
            }
            catch (Exception ex)
            {
                loginStatus.IsSuccessful = false;
                loginStatus.Message = ex.Message;
            }

            return loginStatus;
        }
        public async Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThan = "")
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

            if (!String.IsNullOrEmpty(olderThan))
                args["olderThan"] = olderThan;
            
            HttpWebRequest request = FormRequest(RequestUtils.API, RequestUtils.POSTS_PATH, args);
            RequestStatus requestStatus = new RequestStatus();

            Debug.WriteLine("TOKEN: " + _token);
            Debug.WriteLine("APP_ID: " + _generatedAppId);
            Debug.WriteLine(request.Address.ToString());

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseText = await reader.ReadToEndAsync();

                    Debug.WriteLine("");
                    Debug.WriteLine(responseText);

                    requestStatus = ValidateResponse(responseText);

                    if (requestStatus.IsSuccessful)
                    {
                        Posts = new List<Post>();

                        var jsonData = JObject.Parse(responseText);
                        var rawPosts = jsonData["data"]["posts"];
                        
                        foreach (var item in rawPosts)
                        {
                            Post post = item.ToObject<Post>();
                            string url = string.Empty;

                            switch (post.Type)
                            {
                                case PostType.Photo:
                                    url = item["images"]["image700"]["url"].ToString();
                                    break;
                                case PostType.Animated:
                                    url = item["images"]["image460sv"]["url"].ToString();
                                    break;
                                case PostType.Video:
                                    url = item["videoId"].ToString();
                                    break;
                                default:
                                    break;
                            }
                            
                            //post.PostMedia = PostMediaFactory.CreatePostMedia(PostType.Video, "bF9zOykErJ4");
                            post.PostMedia = PostMediaFactory.CreatePostMedia(post.Type, url);
                            Posts.Add(post);
                            //break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.IsSuccessful = false;
                requestStatus.Message = ex.Message;
            }

            return requestStatus;
        }
        public async Task<RequestStatus> GetCommentsAsync(string postUrl, uint count)
        {
            var args = new Dictionary<string, string>();

            string path = "v1/topComments.json?" +
                "appId=a_dd8f2b7d304a10edaf6f29517ea0ca4100a43d1b" +
                "&urls=" + postUrl +
                "&commentL1=" + count +
                "&pretty=0";

            HttpWebRequest request = FormRequest(RequestUtils.COMMENT_CDN, path, args);
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync()))
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseText = await reader.ReadToEndAsync();
                    requestStatus = ValidateResponse(responseText);

                    if (requestStatus.IsSuccessful)
                    {
                        Comments = new List<Comment>();
                        Debug.WriteLine(responseText);
                        var jsonData = JObject.Parse(responseText);
                        var comments = jsonData["payload"]["data"][0]["comments"];

                        foreach (var item in comments)
                        {
                            Comment comment = item.ToObject<Comment>();                            
                            Comments.Add(comment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.IsSuccessful = false;
                requestStatus.Message = ex.Message;
            }

            return requestStatus;
        }

        #endregion

        #region Implementation

        private HttpWebRequest FormRequest(string api, string path, Dictionary<string, string> args)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "9GAG-9GAG_TOKEN", _token },
                { "9GAG-TIMESTAMP", _timestamp },
                { "9GAG-APP_ID", _appId },
                { "X-Package-ID", _appId },
                { "9GAG-DEVICE_UUID", _deviceUuid },
                { "X-Device-UUID", _deviceUuid },
                { "9GAG-DEVICE_TYPE", "android" },
                { "9GAG-BUCKET_NAME", "MAIN_RELEASE" },
                { "9GAG-REQUEST-SIGNATURE", _signature }
            };

            List<string> argsStrings = new List<string>();

            foreach (KeyValuePair<string, string> entry in args)
            {
                argsStrings.Add(String.Format("{0}/{1}", entry.Key, entry.Value));
            }

            List<string> urlItems = new List<string>()
            {
                api,
                path,
                String.Join("/", argsStrings)
            };
            string url = String.Join("/", urlItems);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var headerCollection = new WebHeaderCollection();

            foreach (KeyValuePair<string, string> entry in headers)
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
            RequestStatus requestStatus = new RequestStatus();

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
            }

            return requestStatus;
        }

        #endregion

        #region Fields

        private string _timestamp = "";
        private string _appId = "";
        private string _token = "";
        private string _deviceUuid = "";
        private string _signature = "";
        private string _userData = "";
        private string _generatedAppId = "";

        #endregion
    }
}
