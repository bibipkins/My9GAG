using Xamarin.Forms;

namespace My9GAG.Models.Post.Media
{
    public interface IPostMedia
    {
        #region Properties

        View View { get; }
        PostType Type { get; }
        string Url { get; set; }
        double Width { get; set; }
        double Height { get; set; }

        #endregion

        #region Methods

        void GenerateView();
        void Start();
        void Stop();
        void Pause();
        void Reload();
        void Unload();

        #endregion
    }
}
