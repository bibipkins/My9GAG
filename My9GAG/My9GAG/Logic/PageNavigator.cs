using System;

namespace My9GAG.Logic
{
    public class PageNavigator : IPageNavigator
    {
        #region Actions

        public Action OnOpenPostsPage;
        public Action OnOpenCommentsPage;
        public Action OnOpenLoginPage;

        #endregion

        #region Methods

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

        #endregion
    }
}
