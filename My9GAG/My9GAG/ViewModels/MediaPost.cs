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
            ID = post.ID;
            Title = post.Title;
            URL = post.URL;
            UpvoteCount = post.UpvoteCount;
            CommentsCount = post.CommentsCount;
            MediaURL = post.MediaURL;
            Type = post.Type;

            switch (Type)
            {
                case PostType.Photo:
                    MediaView = new ZoomableImage() { Source = MediaURL };
                    break;
                case PostType.Animated:
                    MediaView = new VideoPlayer();
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
                    photo.Source = MediaURL;
                    break;
                case PostType.Animated:
                    var video = MediaView as VideoPlayer;
                    if (video == null) return;
                    video.Source = MediaURL;
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
    }
}
