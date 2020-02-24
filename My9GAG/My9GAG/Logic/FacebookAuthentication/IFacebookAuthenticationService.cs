
namespace My9GAG.Logic.FacebookAuthentication
{
    public interface IFacebookAuthenticationService
    {
        string GetAuthenticationPageUrl(string state);
    }
}
