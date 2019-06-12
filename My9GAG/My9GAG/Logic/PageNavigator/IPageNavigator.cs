
namespace My9GAG.Logic.PageNavigator
{
    public interface IPageNavigator
    {
        void GoBack();
        void OpenPostsPage();
        void OpenCommentsPage();
        void OpenLoginPage();
        void OpenLoginWithGooglePage();
        void OpenLoginWithFacebookPage();
        void OpenRegistrationPage();
    }
}
