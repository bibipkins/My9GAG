using Newtonsoft.Json.Linq;

namespace My9GAG.Models.Post.Media
{
    public static class PostMediaFactory
    {
        #region Methods

        public static IPostMedia CreatePostMedia(PostType type, JToken jsonObject)
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

        public static ImagePostMedia CreateImagePostMedia(JToken jsonObject)
        {
            return jsonObject["images"]["image700"].ToObject<ImagePostMedia>();
        }
        public static AnimatedPostMedia CreateAnimatedPostMedia(JToken jsonObject)
        {
            return jsonObject["images"]["image460sv"].ToObject<AnimatedPostMedia>();
        }
        public static TextPostMedia CreateConversionErrorPostMedia()
        {
            return new TextPostMedia()
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
