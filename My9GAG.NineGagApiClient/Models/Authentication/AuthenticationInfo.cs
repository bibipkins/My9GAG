using My9GAG.Models.Authentication;
using My9GAG.NineGagApiClient.Utils;
using System;

namespace My9GAG.NineGagApiClient.Models.Authentication
{
    public class AuthenticationInfo
    {
        #region Constructors

        public AuthenticationInfo()
        {
            Timestamp = RequestUtils.GetTimestamp();
            Token = RequestUtils.GetSha1(Timestamp);
            DeviceUuid = RequestUtils.GetUuid();
            Signature = RequestUtils.GetSignature(Timestamp, AppId, DeviceUuid);
            TokenWillExpireAt = DateTime.UtcNow;
            LastAuthenticationType = AuthenticationType.None;
        }

        #endregion

        #region Properties

        public string UserLogin
        {
            get;
            set;
        }
        public string UserPassword
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }
        public string Timestamp
        {
            get;
            set;
        }
        public string AppId
        {
            get => "com.ninegag.android.app";
        }
        public string DeviceUuid
        {
            get;
            set;
        }
        public string Signature
        {
            get;
            set;
        }

        public DateTime TokenWillExpireAt
        {
            get;
            set;
        }
        public AuthenticationType LastAuthenticationType { get; set; }
        public bool HasTokenExpired
        {
            get => TokenWillExpireAt <= DateTime.UtcNow;
        }
        public bool IsAuthenticated
        {
            get => !(string.IsNullOrEmpty(Token) || HasTokenExpired);
        }
        public bool AreCredentialsPresent
        {
            get => !(string.IsNullOrEmpty(UserLogin) || string.IsNullOrEmpty(UserPassword));
        }

        #endregion

        #region Methods

        public void ClearToken()
        {
            Token = string.Empty;
            TokenWillExpireAt = DateTime.UtcNow;
            LastAuthenticationType = AuthenticationType.None;
        }

        #endregion
    }
}
