using My9GAG.Views.CustomViews.VideoPlayer;

namespace My9GAG.Models
{
    public class VideoPostView : PostView
    {
        #region Methods

        public override void Start()
        {
            (View as VideoPlayer).Play();
        }
        public override void Stop()
        {
            (View as VideoPlayer).Stop();
        }
        public override void Pause()
        {
            (View as VideoPlayer).Pause();
        }

        public override void GenerateView()
        {
            View = new VideoPlayer()
            {
                Source = new UriVideoSource()
                {
                    Uri = PostMedia.Url
                }
            };
        }
        public override void Load()
        {
            (View as VideoPlayer).Source = new UriVideoSource()
            {
                Uri = PostMedia.Url
            };
        }
        public override void Unload()
        {
            (View as VideoPlayer).Source = null;
        }
        
        #endregion
    }
}
