using System;

namespace My9GAG.Logic.PageNavigator
{
    public class PageNavigator : IPageNavigator
    {
        #region Actions

        public Action OnGoBack;
        public Action OnOpenPostsPage;
        public Action OnOpenCommentsPage;
        public Action OnOpenLoginPage;
        public Action OnOpenLoginWithGooglePage;
        public Action OnOpenLoginWithFacebookPage;

        #endregion

        #region Methods

        public void GoBack()
        {
            OnGoBack?.Invoke();
        }
        public void OpenPostsPage()
        {
            OnOpenPostsPage?.Invoke();
        }
        public void OpenCommentsPage()
        {
            OnOpenCommentsPage?.Invoke();
        }
        public void OpenLoginPage()
        {
            OnOpenLoginPage?.Invoke();
        }
        public void OpenLoginWithGooglePage()
        {
            OnOpenLoginWithGooglePage?.Invoke();
        }
        public void OpenLoginWithFacebookPage()
        {
            OnOpenLoginWithFacebookPage?.Invoke();
        }

        #endregion
    }
}
