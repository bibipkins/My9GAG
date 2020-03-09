using My9GAG.Logic.Request;
using NineGagApiClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My9GAG.Logic.Client
{
    public interface IClientService
    {
        #region Properties

        IList<Post> Posts { get; }
        IList<Comment> Comments { get; }
        AuthenticationInfo AuthenticationInfo { get; }

        #endregion

        #region Methods

        Task<RequestStatus> LoginWithCredentialsAsync(string userName, string password);
        Task<RequestStatus> LoginWithGoogleAsync(string token);
        Task<RequestStatus> LoginWithFacebookAsync(string token);
        Task Logout();

        Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThanPostId = "");
        Task<RequestStatus> GetCommentsAsync(string postUrl, int count);

        Task LoadAuthenticationInfoAsync();
        Task SaveAuthenticationInfoAsync();

        #endregion
    }
}
