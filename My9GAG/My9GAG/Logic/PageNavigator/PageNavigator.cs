using My9GAG.ViewModels;
using System;
using Xamarin.Forms;

namespace My9GAG.Logic.PageNavigator
{
    /// <summary>
    /// All of the navigational oprations should be wrapped in Device.BeginInvokeOnMainThread()
    /// in order to avoid exceptions as ui-related operations must be performed on ui-thread
    /// </summary>
    public class PageNavigator : IPageNavigator
    {
        #region Actions

        public Action OnGoBack;
        public Action<PostsPageViewModel, bool> OnOpenPostsPage;
        public Action<CommentsPageViewModel, bool> OnOpenCommentsPage;
        public Action<LoginPageViewModel, bool> OnOpenLoginPage;
        public Action<LoginWithGooglePageViewModel, bool> OnOpenLoginWithGooglePage;
        public Action<LoginWithFacebookPageViewModel, bool> OnOpenLoginWithFacebookPage;
        public Action<RegistrationPageViewModel, bool> OnOpenRegistrationPage;

        #endregion

        #region Methods

        public void GoBack()
        {
            Device.BeginInvokeOnMainThread(() => 
                OnGoBack?.Invoke());
        }
        public void GoToPostsPage(PostsPageViewModel viewModel, bool canGoBack)
        {
            Device.BeginInvokeOnMainThread(() => 
                OnOpenPostsPage?.Invoke(viewModel, canGoBack));
        }
        public void GoToCommentsPage(CommentsPageViewModel viewModel, bool canGoBack)
        {
            Device.BeginInvokeOnMainThread(() => 
                OnOpenCommentsPage?.Invoke(viewModel, canGoBack));
        }
        public void GoToLoginPage(LoginPageViewModel viewModel, bool canGoBack)
        {
            Device.BeginInvokeOnMainThread(() => 
                OnOpenLoginPage?.Invoke(viewModel, canGoBack));
        }
        public void GoToLoginWithGooglePage(LoginWithGooglePageViewModel viewModel, bool canGoBack)
        {
            Device.BeginInvokeOnMainThread(() => 
                OnOpenLoginWithGooglePage?.Invoke(viewModel, canGoBack));
        }
        public void GoToLoginWithFacebookPage(LoginWithFacebookPageViewModel viewModel, bool canGoBack)
        {
            Device.BeginInvokeOnMainThread(() => 
                OnOpenLoginWithFacebookPage?.Invoke(viewModel, canGoBack));
        }
        public void GoToRegistrationPage(RegistrationPageViewModel viewModel, bool canGoBack)
        {
            Device.BeginInvokeOnMainThread(() => 
                OnOpenRegistrationPage?.Invoke(viewModel, canGoBack));
        }

        #endregion
    }
}
