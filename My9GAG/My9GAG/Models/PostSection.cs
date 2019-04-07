using Newtonsoft.Json;

namespace My9GAG.Models
{
    public class PostSection
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
    }
}
