using My9GAG.Models.Comment;
using System.Collections.ObjectModel;

namespace My9GAG.ViewModels
{
    public class CommentsPageViewModel : ViewModelBase
    {
        #region Constructor

        public CommentsPageViewModel()
        {
            Comments = new ObservableCollection<Comment>();
        }

        #endregion

        #region Properties

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public ObservableCollection<Comment> Comments
        {
            get
            {
                return _comments;
            }
            private set
            {
                SetProperty(ref _comments, value);
            }
        }

        #endregion

        #region Fields

        private string _title;
        private ObservableCollection<Comment> _comments;

        #endregion
    }
}
