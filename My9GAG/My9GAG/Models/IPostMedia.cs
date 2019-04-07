using Xamarin.Forms;

namespace My9GAG.Models
{
    public interface IPostMedia
    {
        string Url { get; set; }
        View View { get; }
        PostType Type { get; }

        void Start();
        void Stop();
        void Pause();
        void Reload();
    }
}
