using Newtonsoft.Json;

namespace My9GAG.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "userId")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "avatarUrl")]
        public string UserAvatar { get; set; }
    }
}
