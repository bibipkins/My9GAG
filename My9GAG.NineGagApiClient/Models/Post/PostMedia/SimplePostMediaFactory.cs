using Newtonsoft.Json.Linq;

namespace My9GAG.Models.Post.Media
{
    public static class SimplePostMediaFactory
    {
        #region Methods

        public static ISimplePostMedia CreatePostMedia(PostType type, JToken jsonObject)
        {
            switch (type)
            {
                case PostType.Photo:
                    return CreateImagePostMedia(jsonObject);
                case PostType.Animated:
                    return CreateAnimatedPostMedia(jsonObject);
                default:
                    return CreateConversionErrorPostMedia();
            }
        }

        public static SimpleImagePostMedia CreateImagePostMedia(JToken jsonObject)
        {
            return jsonObject["images"]["image700"].ToObject<SimpleImagePostMedia>();
        }
        public static SimpleAnimatedPostMedia CreateAnimatedPostMedia(JToken jsonObject)
        {
            return jsonObject["images"]["image460sv"].ToObject<SimpleAnimatedPostMedia>();
        }
        public static SimpleTextPostMedia CreateConversionErrorPostMedia()
        {
            return new SimpleTextPostMedia()
            {
                Text = CONVERSION_ERROR_MESSAGE
            };
        }

        #endregion

        #region Constants

        private const string CONVERSION_ERROR_MESSAGE = "Could not convert this post to any of existing media views";

        #endregion
    }
}
