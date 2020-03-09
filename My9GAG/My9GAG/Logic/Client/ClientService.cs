using My9GAG.Logic.Logger;
using My9GAG.Logic.Request;
using My9GAG.Logic.SecureStorage;
using Newtonsoft.Json.Linq;
using NineGagApiClient;
using NineGagApiClient.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My9GAG.Logic.Client
{
    public class ClientService : ApiClient, IClientService
    {
        #region Constructors

        public ClientService(ILogger logger, ISecureStorage secureStorage) : base()
        {
            _logger = logger;
            _secureStorage = secureStorage;

            Posts = new List<Post>();
            Comments = new List<Comment>();
        }

        #endregion

        #region Properties

        public IList<Post> Posts
        {
            get;
            private set;
        }
        public IList<Comment> Comments
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        public new async Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThanPostId = "")
        {
            return await ExecuteAndReturnRequestStatusAsync(async () =>
                Posts = await base.GetPostsAsync(postCategory, count, olderThanPostId));
        }
        public new async Task<RequestStatus> GetCommentsAsync(string postUrl, int count)
        {
            return await ExecuteAndReturnRequestStatusAsync(async () =>
                Comments = await base.GetCommentsAsync(postUrl, count));
        }

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
        public new async Task Logout()
        {
            base.Logout();
            await SaveAuthenticationInfoAsync();
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

        #endregion

        #region Implementation

        private async Task<RequestStatus> ExecuteAndReturnRequestStatusAsync(Func<Task> func)
        {
            try
            {
                await func();

                return new RequestStatus
                { 
                    IsSuccessful = true
                };
            }
            catch (Exception e)
            {
                _logger.Log(e.Message);

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

        #endregion
    }
}
