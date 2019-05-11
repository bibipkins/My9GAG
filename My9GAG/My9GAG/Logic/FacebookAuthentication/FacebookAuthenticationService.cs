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
                "&redirect_uri=" + REDIRECT_URL;

            return url;
        }

        //public async Task<string> GetAccessTokenAsync(string code)
        //{
        //    string requestUrl = "https://www.googleapis.com/oauth2/v4/token"
        //                 + "?code=" + code
        //                 + "&client_id=" + CLIENT_ID
        //                 + "&client_secret=" + CLIENT_SECRET
        //                 + "&redirect_uri=" + System.Net.WebUtility.UrlEncode(REDIRECT_URL)
        //                 + "&grant_type=authorization_code";

        //    var client = new HttpClient();
        //    var response = await client.PostAsync(requestUrl, null);
        //    var data = await response.Content.ReadAsStringAsync();

        //    return JsonConvert.DeserializeObject<JObject>(data).Value<string>(JSON_ACCESS_TOKEN_KEY);
        //}

        #endregion

        #region Constants

        private const string AUTHENTICATION_PAGE_BASE_URL = "https://www.facebook.com/v3.3/dialog/oauth";        
        private const string REDIRECT_URL = "https://9gag.com/";
        private const string CLIENT_ID = "2590641477631935";

        #endregion
    }
}
