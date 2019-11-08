using System;
using System.Collections.Generic;
using System.Text;

namespace My9GAG.Logic.FacebookAuthentication
{
    public interface IFacebookAuthenticationService
    {
        string GetAuthenticationPageUrl(string state);
    }
}
