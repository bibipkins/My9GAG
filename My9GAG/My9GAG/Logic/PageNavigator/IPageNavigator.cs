using My9GAG.ViewModels;

namespace My9GAG.Logic.PageNavigator
{
    public interface IPageNavigator
    {
        void GoBack();
        void GoToPostsPage(PostsPageViewModel viewModel = null, bool canGoBack = true);
        void GoToCommentsPage(CommentsPageViewModel viewModel = null, bool canGoBack = true);
        void GoToLoginPage(LoginPageViewModel viewModel = null, bool canGoBack = true);
        void GoToLoginWithGooglePage(LoginWithGooglePageViewModel viewModel = null, bool canGoBack = true);
        void GoToLoginWithFacebookPage(LoginWithFacebookPageViewModel viewModel = null, bool canGoBack = true);
        void GoToRegistrationPage(RegistrationPageViewModel viewModel = null, bool canGoBack = true);
    }
}
