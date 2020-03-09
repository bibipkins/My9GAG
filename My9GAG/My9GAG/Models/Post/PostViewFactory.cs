using NineGagApiClient.Models;

namespace My9GAG.Models
{
    public static class PostViewFactory
    {
        #region Methods

        public static PostView CreatePostViewFromPost(Post post)
        {
            PostView postView = null;

            switch (post.Type)
            {
                case PostType.Photo:
                    postView = new ImagePostView();
                    break;
                case PostType.Animated:
                    postView = new VideoPostView();
                    break;
                default:
                    postView = new TextPostView() { Text = CONVERSION_ERROR_MESSAGE };
                    break;
            }

            postView.Id = post.Id;
            postView.Title = post.Title;
            postView.Url = post.Url;
            postView.UpvoteCount = post.UpvoteCount;
            postView.DownvoteCount = post.DownvoteCount;
            postView.CommentsCount = post.CommentsCount;
            postView.PostMedia = post.PostMedia;
            postView.Type = post.Type;
            postView.Section = post.Section;
            postView.IsNsfw = post.IsNsfw;

            postView.GenerateView();

            return postView;
        }

        #endregion

        #region Constants

        private const string CONVERSION_ERROR_MESSAGE = "Could not convert this post to any of existing media views";

        #endregion
    }
}
