using System;

namespace My9GAG.Views.CustomViews.VideoPlayer
{
    public interface IVideoPlayer
    {
        VideoPlayerStatus Status { get; set; }
        TimeSpan Duration { get; set; }
    }
}
