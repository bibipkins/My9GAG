using Newtonsoft.Json;
using System;

namespace My9GAG.Models.User
{
    public class User
    {
        #region Constructors

        public User()
        {
            LoginStatus = LoginStatus.None;
            TokenExpirationTime = new DateTime();
        }

        #endregion

        #region Methods

        public bool TokenExpired()
        {
            return DateTime.UtcNow.Ticks >= TokenExpirationTime.Ticks;
        }

        #endregion

        #region Properties

        [JsonProperty(PropertyName = "userId")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "avatarUrl")]
        public string UserAvatar { get; set; }

        public string Login { get; set; }
        public string Password { get; set; }
        public LoginStatus LoginStatus { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpirationTime { get; set; }

        #endregion
    }
}
