using My9GAG.Models.Authentication;

namespace My9GAG.Logic.FacebookAuthentication
{
    public class FacebookAuthenticationService : IFacebookAuthenticationService
    {
        #region Methods

        public string GetAuthenticationPageUrl(string state)
        {
            string url = AUTHENTICATION_PAGE_BASE_URL +
                "?client_id=" + AuthenticationSecrets.FacebookClientId +
                "&response_type=token" +
                "&state=" + state +
                "&scope=email" +
                "&redirect_uri=" + REDIRECT_URL;

            return url;
        }

        #endregion

        #region Constants

        //private const string AUTHENTICATION_PAGE_BASE_URL = "https://www.facebook.com/v3.3/dialog/oauth";
        private const string AUTHENTICATION_PAGE_BASE_URL = "https://m.facebook.com/v2.12/dialog/oauth";
        private const string REDIRECT_URL = "https://9gag.com/";
        
        #endregion
    }
}
