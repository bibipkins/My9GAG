using My9GAG.Models;

namespace My9GAG.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        public LoginPageViewModel()
        {
            User = new User();
        }

        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                SetProperty(ref _user, value);
            }
        }

        private User _user;
    }
}
