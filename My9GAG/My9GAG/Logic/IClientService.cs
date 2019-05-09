using My9GAG.Models.Comment;
using My9GAG.Models.Post;
using My9GAG.Models.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace My9GAG.Logic
{
    public interface IClientService
    {
        Task<RequestStatus> LoginAsync(string userName, string password);
        Task<RequestStatus> LoginWithGoogleAsync(string token);
        Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThan = "");
        Task<RequestStatus> GetCommentsAsync(string postUrl, uint count);

        List<Post> Posts { get; }
        List<Comment> Comments { get; }
    }
}
