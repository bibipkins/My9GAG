using My9GAG.Models.Authentication;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace My9GAG.Logic.GoogleAuthentication
{
    public class GoogleAuthenticationService : IGoogleAuthenticationService
    {
        #region Methods

        public string GetAuthenticationPageUrl()
        {
            string url = AUTHENTICATION_PAGE_BASE_URL 
                + "?response_type=code"
                + "&scope=openid"
                + "&redirect_uri=" + REDIRECT_URL
                + "&client_id=" + AuthenticationSecrets.GoogleClientId;

            return url;
        }
        public async Task<string> GetAccessTokenAsync(string code)
        {
            string requestUrl = "https://www.googleapis.com/oauth2/v4/token"
                + "?code=" + code
                + "&client_id=" + AuthenticationSecrets.GoogleClientId
                + "&client_secret=" + AuthenticationSecrets.GoogleClientSecret
                + "&redirect_uri=" + System.Net.WebUtility.UrlEncode(REDIRECT_URL)
                + "&grant_type=authorization_code";

            var client = new HttpClient();
            var response = await client.PostAsync(requestUrl, null);
            var data = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<JObject>(data).Value<string>(JSON_ACCESS_TOKEN_KEY);
        }

        #endregion

        #region Constants

        private const string AUTHENTICATION_PAGE_BASE_URL = "https://accounts.google.com/o/oauth2/v2/auth";
        private const string REDIRECT_URL = "https://9gag.com/";
        private const string JSON_ACCESS_TOKEN_KEY = "access_token";

        #endregion
    }
}
