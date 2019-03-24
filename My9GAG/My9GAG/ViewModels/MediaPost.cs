using My9GAG.Models;
using My9GAG.Views;
using System.Collections.Generic;
using Xamarin.Forms;

namespace My9GAG.ViewModels
{
    public class MediaPost : Post
    {
        public MediaPost(Post post)
        {
            Id = post.Id;
            Title = post.Title;
            Url = post.Url;
            UpvoteCount = post.UpvoteCount;
            CommentCount = post.CommentCount;
            MediaUrl = post.MediaUrl;
            Type = post.Type;

            switch (Type)
            {
                case PostType.Photo:
                    MediaView = new ZoomableImage() { Source = MediaUrl };
                    break;
                case PostType.Animated:
                    MediaView = new VideoPlayer();
                    break;
                case PostType.Video:
                    var html = new HtmlWebViewSource
                    {
                        Html = GenerateHtml(MediaUrl)
                    };
                    MediaView = new WebView() { Source = html };
                    break;
                default:
                    MediaView = new Label()
                    {
                        Text = "Could not convert this post to any media view",
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        BackgroundColor = Color.Black,
                        TextColor = Color.Red
                    };
                    break;
            }
        }

        public void Start()
        {
            if (MediaView is VideoPlayer view)
                view.Start();
        }
        public void Stop()
        {
            if (MediaView is VideoPlayer view)
                view.Stop();
        }
        public void Pause()
        {
            if (MediaView is VideoPlayer view)
                view.Pause();
        }
        public void Reload()
        {
            switch (Type)
            {
                case PostType.Photo:
                    var photo = MediaView as ZoomableImage;
                    if (photo == null) return;
                    photo.Source = MediaUrl;
                    break;
                case PostType.Animated:
                    var video = MediaView as VideoPlayer;
                    if (video == null) return;
                    video.Source = MediaUrl;
                    break;
            }
        }

        public View MediaView { get; set; }

        public static MediaPost Convert(Post post)
        {
            MediaPost mediaPost = new MediaPost(post);
            return mediaPost;
        }
        public static IEnumerable<MediaPost> Convert(IEnumerable<Post> posts)
        {
            List<MediaPost> mediaPosts = new List<MediaPost>();

            foreach (var post in posts)
                mediaPosts.Add(new MediaPost(post));

            return mediaPosts;
        }

        private string GenerateHtml(string ytId)
        {
            return "<html><body><iframe width=\"100%\" height=\"100%\" src=\"https://www.youtube.com/embed/" + ytId + "?controls=0\" frameborder = \"0\" allow = \"accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture\" allowfullscreen></iframe></body><style>body{background:black;margin:0;padding:0;border:0;}</style></html>";
        }
    }
}
