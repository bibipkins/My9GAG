
namespace My9GAG.Models.Post.Media
{
    //We call these simple so we don't conflict with the class names in the My9GAG project
    //The reason we use the same namespace as the My9GAG project is for backward compatibility
    //and because I can't refactor the UWP, IOS and Android projects (missing plugins)
    public interface ISimplePostMedia
    {
        #region Properties

        PostType Type { get; }
        string Url { get; set; }
        double Width { get; set; }
        double Height { get; set; }

        #endregion
    }
}
