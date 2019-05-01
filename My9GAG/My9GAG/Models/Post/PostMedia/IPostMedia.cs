using Xamarin.Forms;

namespace My9GAG.Models
{
    public interface IPostMedia
    {
        #region Properties

        View View { get; }
        PostType Type { get; }
        string Url { get; set; }

        #endregion

        #region Methods

        void Start();
        void Stop();
        void Pause();
        void Reload();
        void Unload();

        #endregion
    }
}
