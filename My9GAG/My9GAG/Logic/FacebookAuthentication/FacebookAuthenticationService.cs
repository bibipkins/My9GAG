using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace My9GAG.Logic.FacebookAuthentication
{
    public class FacebookAuthenticationService : IFacebookAuthenticationService
    {
        #region Methods

        public string GetAuthenticationPageUrl(string state)
        {
            string url = AUTHENTICATION_PAGE_BASE_URL +
                "?client_id=" + CLIENT_ID +
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
        private const string CLIENT_ID = "2590641477631935";

        #endregion
    }
}
