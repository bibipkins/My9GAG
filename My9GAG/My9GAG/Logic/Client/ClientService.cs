using My9GAG.Logic.Logger;
using My9GAG.Logic.Request;
using My9GAG.Logic.SecureStorage;
using My9GAG.Models.Authentication;
using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.Models.Post.Media;
using My9GAG.NineGagApiClient.Models.Authentication;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace My9GAG.Logic.Client
{
    public class ClientService : My9GAG.NineGagApiClient.ApiClient, IClientService
    {
        #region Constructors

        public ClientService(ILogger logger, ISecureStorage secureStorage, bool generatePostMediaOnLoad = true)
            : base()
        {
            _logger = logger;
            _secureStorage = secureStorage;
            _generatePostMediaOnLoad = generatePostMediaOnLoad;

            Posts = new List<Post>();
            Comments = new List<Comment>();
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

        AuthenticationInfo IClientService.AuthenticationInfo => (AuthenticationInfo)this.AuthenticationInfo;
        #endregion

        #region Methods

        #region ApiFunctionality

        public new async Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, 
            string olderThanPostId = "")
        {
            return await ExecuteAndReturnRequestStatusAsync(async () =>
            {
                var posts = await base.GetPostsAsync(postCategory, count, olderThanPostId);
                Posts = posts.Cast<Post>().ToList();
            });
        }

        protected override SimplePost CreatePost(JToken item)
        {
            var post = item.ToObject<Post>();
            post.PostMedia = PostMediaFactory.CreatePostMedia(post.Type, item);
            if (_generatePostMediaOnLoad)
            {
                post.PostMedia.GenerateView();
            }
            return post;
        }

        async Task<RequestStatus> IClientService.GetCommentsAsync(string postUrl, int count)
        {
            return await ExecuteAndReturnRequestStatusAsync(async () =>
            {
                await base.GetCommentsAsync(postUrl, count);
            });
        }
        #endregion

        #region Auth

        public new async Task<RequestStatus> LoginWithCredentialsAsync(string userName, string password)
        {
            return await ExecuteAndReturnRequestStatusAsync(async () =>
            {
                await base.LoginWithCredentialsAsync(userName, password);
                await SaveAuthenticationInfoAsync();
            });
        }

        public new async Task<RequestStatus> LoginWithGoogleAsync(string token)
        {
            return await ExecuteAndReturnRequestStatusAsync(async () =>
            {
                await base.LoginWithGoogleAsync(token);
                await SaveAuthenticationInfoAsync();
            });
        }

        public new async Task<RequestStatus> LoginWithFacebookAsync(string token)
        {
            return await ExecuteAndReturnRequestStatusAsync(async () =>
            {
                await base.LoginWithFacebookAsync(token);
                await SaveAuthenticationInfoAsync();
            });
        }

        public async Task SaveAuthenticationInfoAsync()
        {
            string authenticationInfoJson = JObject.FromObject(AuthenticationInfo).ToString();
            await _secureStorage.SetAsync("AuthenticationInfo", authenticationInfoJson);
        }

        public async Task LoadAuthenticationInfoAsync()
        {
            var result = await _secureStorage.GetAsync("AuthenticationInfo");

            if (result.IsSuccessful)
            {
                var jsonData = JObject.Parse(result.Value);
                AuthenticationInfo = jsonData.ToObject<AuthenticationInfo>();
            }
        }
        public new async Task Logout()
        {
            base.Logout();
            await SaveAuthenticationInfoAsync();
        } 
        #endregion

        public void SaveState(IDictionary<string, object> dictionary)
        {

        }

        public void RestoreState(IDictionary<string, object> dictionary)
        {

        }

        private async Task<RequestStatus> ExecuteAndReturnRequestStatusAsync(Func<Task> func)
        {
            try
            {
                await func();
                return new RequestStatus { IsSuccessful = true };
            }
            catch (Exception e)
            {
                _logger?.Log(func.ToString());
                return new RequestStatus
                {
                    IsSuccessful = false,
                    Message = e.Message
                };
            }
        }

        #endregion

        #region Fields
        private readonly ILogger _logger;
        private readonly ISecureStorage _secureStorage;
        private readonly bool _generatePostMediaOnLoad;
        #endregion
    }
}
