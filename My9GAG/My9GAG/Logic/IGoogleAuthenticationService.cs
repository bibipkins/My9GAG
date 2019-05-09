using System.Threading.Tasks;

namespace My9GAG.Logic
{
    public interface IGoogleAuthenticationService
    {
        string GetAuthenticationPageUrl();
        Task<string> GetAccessTokenAsync(string code);        
    }
}
