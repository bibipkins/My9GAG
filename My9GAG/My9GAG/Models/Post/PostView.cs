using NineGagApiClient.Models;
using Xamarin.Forms;

namespace My9GAG.Models
{
    public abstract class PostView : Post
    {
        public View View { get; protected set; }

        public virtual void Start()
        {
            // not every view needs to start
        }
        public virtual void Stop()
        {
            // not every view needs to stop
        }
        public virtual void Pause()
        {
            // not every view needs to pause
        }

        public abstract void GenerateView();
        public abstract void Load();
        public abstract void Unload();
    }
}
