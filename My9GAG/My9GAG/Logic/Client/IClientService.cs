using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.Models.Request;
using My9GAG.Models.User;
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
        Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThan = "");
        Task<RequestStatus> GetCommentsAsync(string postUrl, uint count);
        void SaveState(IDictionary<string, object> dictionary);
        void RestoreState(IDictionary<string, object> dictionary);

        #endregion

        #region Properties

        List<Post> Posts { get; }
        List<Comment> Comments { get; }
        User User { get; }

        #endregion
    }
}
