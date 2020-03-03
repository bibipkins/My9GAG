using My9GAG.Logic.Request;
using My9GAG.NineGagApiClient.Models.Authentication;
using System;

namespace My9GAG.Models.Authentication
{
    public class AuthenticationInfo : SimpleAuthenticationInfo
    {
        #region Constructors

        public AuthenticationInfo() : base()
        {
            LastAuthenticationType = AuthenticationType.None;
        }

        #endregion

        #region Properties

        public AuthenticationType LastAuthenticationType
        {
            get;
            set;
        }

        #endregion
    }
}
