using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;

namespace My9GAG.Models
{
    class Client
    {
        #region Constructors

        public Client()
        {
            this.timestamp  = RequestUtils.GetTimestamp();
            this.appID      = RequestUtils.APP_ID;
            this.token      = RequestUtils.GetSHA1(timestamp);
            this.deviceUUID = RequestUtils.GetUUID();
            this.signature  = RequestUtils.GetSignature(timestamp, appID, deviceUUID);

            Posts = new List<Post>();
            Comments = new List<Comment>();
        }

        #endregion

        #region Methods

        public RequestStatus Login(string userName, string password)
        {
            Dictionary<string, string> args = new Dictionary<string, string>()
            {
                { "loginMethod", "9gag" },
                { "loginName", userName },
                { "password", RequestUtils.GetMD5(password) },
                { "language", "en_US" },
                { "pushToken", token }
            };

            HttpWebRequest request = FormRequest(RequestUtils.API, "v2/user-token", args);
            RequestStatus loginStatus = new RequestStatus();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseText = reader.ReadToEnd();
                    loginStatus = ValidateResponse(responseText);

                    if (loginStatus.IsSuccessful)
                    {
                        var jsonData = JObject.Parse(responseText);
                        token = jsonData["data"]["userToken"].ToString();
                        userData = jsonData["data"].ToString();
                        string[] readStateParams = jsonData["data"]["noti"]["readStateParams"].ToString().Split('&');

                        foreach (var param in readStateParams)
                        {
                            string[] pair = param.Split('=');

                            if (pair[0].Contains("appId"))
                            {
                                this.generatedAppId = pair[1];
                                break;
                            }
                        }
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
        public RequestStatus GetPosts(PostCategory postCategory, uint count, string olderThan = "")
        {
            string type = "";

            switch (postCategory)
            {
                case PostCategory.Hot:
                    type = "hot";
                    break;
                case PostCategory.Trending:
                    type = "trending";
                    break;
                case PostCategory.Fresh:
                    type = "vote";
                    break;
                default:
                    type = "hot";
                    break;
            }

            Dictionary<string, string> args = new Dictionary<string, string>()
            {
                { "group", "1" },
                { "type", type },
                { "itemCount", count.ToString() },
                { "entryTypes", "animated,photo,video,album" },
                { "offset", "10" }
            };

            if (!String.IsNullOrEmpty(olderThan))
                args["olderThan"] = olderThan;
            
            HttpWebRequest request = FormRequest(RequestUtils.API, "v2/post-list", args);
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseText = reader.ReadToEnd();
                    requestStatus = ValidateResponse(responseText);

                    if (requestStatus.IsSuccessful)
                    {
                        Posts = new List<Post>();

                        var jsonData = JObject.Parse(responseText);

                        Debug.WriteLine(jsonData.ToString());

                        var rawPosts = jsonData["data"]["posts"];

                        int i = 0;
                        foreach (var item in rawPosts)
                        {
                            Post post = new Post()
                            {
                                ID = item["id"].ToString(),
                                Title = WebUtility.HtmlDecode(item["title"].ToString()),
                                URL = item["url"].ToString(),
                                UpvoteCount = item["upVoteCount"].Value<int>(),
                                CommentsCount = item["commentsCount"].Value<int>(),
                                Type = (PostType)Enum.Parse(typeof(PostType), item["type"].ToString())
                            };

                            //Debug.WriteLine(i + ": " + post.ID + post.Title + post.URL + post.UpvoteCount + CommentsCount + Type);
                            
                            if (post.Type == PostType.Photo)
                                post.MediaURL = item["images"]["image700"]["url"].ToString();
                            else if (post.Type == PostType.Animated)
                                post.MediaURL = item["images"]["image460sv"]["url"].ToString();

                            Posts.Add(post);
                            Debug.WriteLine(i + ": " + Posts[i]);
                            i++;
                        }

                        Debug.WriteLine("103 GetPosts");
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
        public RequestStatus GetComments(string postUrl, uint count)
        {
            Dictionary<string, string> args = new Dictionary<string, string>();

            string path = "v1/topComments.json?" +
                "appId=a_dd8f2b7d304a10edaf6f29517ea0ca4100a43d1b" +
                "&urls=" + postUrl +
                "&commentL1=" + count +
                "&pretty=0";

            HttpWebRequest request = FormRequest(RequestUtils.COMMENT_CDN, path, args);
            RequestStatus requestStatus = new RequestStatus();

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string responseText = reader.ReadToEnd();
                    requestStatus = ValidateResponse(responseText);                    

                    if (requestStatus.IsSuccessful)
                    {
                        Comments = new List<Comment>();

                        var jsonData = JObject.Parse(responseText);
                        var comments = jsonData["payload"]["data"][0]["comments"];

                        foreach (var item in comments)
                        {
                            Comment comment = new Comment()
                            {
                                ID = item["commentId"].ToString(),
                                Text = WebUtility.HtmlDecode(item["text"].ToString()),
                                UserName = item["user"]["displayName"].ToString(),
                                UserAvatar = item["user"]["avatarUrl"].ToString(),
                                LikesCount = item["likeCount"].Value<int>()
                            };

                            Comments.Add(comment);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                requestStatus.IsSuccessful = false;
                requestStatus.Message = ex.Message;
                Debug.WriteLine(ex.Message);
            }

            return requestStatus;
        }

        #endregion

        #region Properties

        public List<Post> Posts { get; private set; }
        public List<Comment> Comments { get; private set; }

        #endregion

        #region Implementation

        private HttpWebRequest FormRequest(string api, string path, Dictionary<string, string> args)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                { "9GAG-9GAG_TOKEN", token },
                { "9GAG-TIMESTAMP", timestamp },
                { "9GAG-APP_ID", appID },
                { "X-Package-ID", appID },
                { "9GAG-DEVICE_UUID", deviceUUID },
                { "X-Device-UUID", deviceUUID },
                { "9GAG-DEVICE_TYPE", "android" },
                { "9GAG-BUCKET_NAME", "MAIN_RELEASE" },
                { "9GAG-REQUEST-SIGNATURE", signature }
            };

            List<string> argsStrings = new List<string>();
            foreach (KeyValuePair<string, string> entry in args)
                argsStrings.Add(String.Format("{0}/{1}", entry.Key, entry.Value));

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
                headerCollection.Add(entry.Key, entry.Value);

            request.Headers = headerCollection;
            request.Method = WebRequestMethods.Http.Get;
            request.UserAgent = ".NET";
            request.ContentType = "application/json";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Accept = "*/*";

            return request;
        }
        private RequestStatus ValidateResponse(string responseText)
        {
            RequestStatus requestStatus = new RequestStatus();
            var jsonData = JObject.Parse(responseText);

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

            return requestStatus;
        }

        #endregion

        #region Fields

        private string timestamp = "";
        private string appID = "";
        private string token = "";
        private string deviceUUID = "";
        private string signature = "";
        private string userData = "";
        private string generatedAppId = "";

        #endregion
    }
}
