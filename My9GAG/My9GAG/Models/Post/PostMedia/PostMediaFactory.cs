
namespace My9GAG.Models
{
    public static class PostMediaFactory
    {
        public static IPostMedia CreatePostMedia(PostType type, string url)
        {
            switch (type)
            {
                case PostType.Photo:
                    return new ImagePostMedia(url);
                case PostType.Animated:
                    return new AnimatedPostMedia(url);
                case PostType.Video:
                    return new YouTubePostMedia(url);
                default:
                    return new TextPostMedia(CONVERSION_ERROR_MESSAGE);
            }
        }

        private const string CONVERSION_ERROR_MESSAGE = "Could not convert this post to any of existing media views";
    }
}
