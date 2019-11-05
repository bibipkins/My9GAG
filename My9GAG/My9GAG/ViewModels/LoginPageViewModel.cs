using My9GAG.Logic.Client;
using My9GAG.Logic.PageNavigator;
using My9GAG.Models.User;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace My9GAG.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region Constructors

        public LoginPageViewModel(IClientService clientService, IPageNavigator pageNavigator)
        {
            _clientService = clientService;
            _pageNavigator = pageNavigator;

            InitCommands();
        }

        #endregion
        
        #region Properties

        public string UserName
        {
            get { return userName; }
            set
            {
                if (SetProperty(ref userName, value))
                {
                    UpdateCommands();
                }
            }
        }
        public string Password
        {
            get { return password; }
            set
            {
                if (SetProperty(ref password, value))
                {
                    UpdateCommands();
                }
            }
        }
        public string LoginErrorMessage
        {
            get { return _loginErrorMessage; }
            set { SetProperty(ref _loginErrorMessage, value); }
        }

        #endregion

        #region Methods

        public async Task LoginAsync()
        {
            StartWorkIndication(ViewModelConstants.LOGIN_MESSAGE);
            
            await Task.Run(async () =>
            {
                var requestStatus = await _clientService.LoginWithCredentialsAsync(userName, password);
                await Task.Delay(ViewModelConstants.LOGIN_DELAY);                

                if (requestStatus.IsSuccessful)
                {
                    _pageNavigator.GoToPostsPage(null, false);
                }
                else
                {
                    LoginErrorMessage = requestStatus.Message;
                }                
            });
            
            StopWorkIndication();
        }

        #endregion

        #region Commands

        public ICommand LoginCommand
        {
            get;
            protected set;
        }
        public ICommand LoginWithGoogleCommand
        {
            get;
            protected set;
        }
        public ICommand LoginWithFacebookCommand
        {
            get;
            protected set;
        }
        public ICommand RegisterCommand
        {
            get;
            protected set;
        }

        public List<ICommand> CommandList
        {
            get;
            protected set;
        }

        #endregion

        #region Implementation

        protected void InitCommands()
        {
            LoginCommand = new Command(
                () => LoginAsync(),
                () => !string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(UserName));
            LoginWithGoogleCommand = new Command(
                () => _pageNavigator.GoToLoginWithGooglePage());
            LoginWithFacebookCommand = new Command(
                () => _pageNavigator.GoToLoginWithFacebookPage());
            RegisterCommand = new Command(
                () => _pageNavigator.GoToRegistrationPage());

            CommandList = new List<ICommand>()
            {
                LoginCommand,
                LoginWithGoogleCommand,
                LoginWithFacebookCommand,
                RegisterCommand
            };
        }
        protected override void UpdateCommands()
        {
            foreach (var command in CommandList)
            {
                if (command is Command c)
                {
                    c.ChangeCanExecute();
                }
            }
        }

        #endregion

        #region Fields

        private User _user;
        private string _loginErrorMessage;
        private IClientService _clientService;
        private IPageNavigator _pageNavigator;

        private string password = "";
        private string userName = "";

        #endregion
    }
}
