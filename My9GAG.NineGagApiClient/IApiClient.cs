using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.NineGagApiClient.Models.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My9GAG.NineGagApiClient
{
    public interface IApiClient
    {
        AuthenticationInfo AuthenticationInfo { get; }

        Task<IList<Comment>> GetCommentsAsync(string postUrl, int count);
        Task<IList<SimplePost>> GetPostsAsync(PostCategory postCategory, int count, string olderThanPostId = "");
        Task LoginWithCredentialsAsync(string userName, string password);
        Task LoginWithFacebookAsync(string token);
        Task LoginWithGoogleAsync(string token);
        void Logout();
    }
}