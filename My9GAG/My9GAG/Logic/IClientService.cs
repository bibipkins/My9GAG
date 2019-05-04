using My9GAG.Models;
using System.Threading.Tasks;

namespace My9GAG.Logic
{
    public interface IClientService
    {
        Task<RequestStatus> LoginAsync(string userName, string password);
        Task<RequestStatus> GetPostsAsync(PostCategory postCategory, int count, string olderThan = "");
        Task<RequestStatus> GetCommentsAsync(string postUrl, uint count);        
    }
}
