using My9GAG.Logic.Request;
using My9GAG.Models.Authentication;
using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.NineGagApiClient.Models.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My9GAG.Logic.Client
{
    public interface IClientService
    {
        #region Methods

        Task<RequestStatus> LoginWithCredentialsAsync(string userName, string password);
        Task<RequestStatus> LoginWithGoogleAsync(string token);
        Task<RequestStatus> LoginWithFacebookAsync(string token);
        Task Logout();

        Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThan = "");
        Task<RequestStatus> GetCommentsAsync(string postUrl, int count);
        Task<RequestStatus> GetGroupsAsync();

        Task LoadAuthenticationInfoAsync();
        Task SaveAuthenticationInfoAsync();

        void SaveState(IDictionary<string, object> dictionary);
        void RestoreState(IDictionary<string, object> dictionary);

        #endregion

        #region Properties

        List<Post> Posts { get; }
        List<Comment> Comments { get; }
        AuthenticationInfo AuthenticationInfo { get; }

        #endregion
    }
}
