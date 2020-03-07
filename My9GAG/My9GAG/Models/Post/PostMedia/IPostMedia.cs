using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public interface IPostMedia : ISimplePostMedia
    {

        void GenerateView();
        void Start();
        void Stop();
        void Pause();
        void Reload();
        void Unload();

    }
}
