using My9GAG.Logic.Client;
using My9GAG.Logic.PageNavigator;
using NineGagApiClient.Models;
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
            LoadAuthenticationInfo();

            var authenticationInfo = _clientService.AuthenticationInfo;

            UserLogin = authenticationInfo.UserLogin;
            UserPassword = authenticationInfo.UserPassword;

            if (authenticationInfo.IsAuthenticated)
            {
                _pageNavigator.GoToPostsPage(null, false);
                return;
            }

            switch (authenticationInfo.LastAuthenticationType)
            {
                case AuthenticationType.Credentials when authenticationInfo.AreCredentialsPresent:
                    LoginAsync();
                    break;
                case AuthenticationType.Google:
                    _pageNavigator.GoToLoginWithGooglePage();
                    break;
                case AuthenticationType.Facebook:
                    _pageNavigator.GoToLoginWithFacebookPage();
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Properties

        public string UserLogin
        {
            get { return _userLogin; }
            set
            {
                if (SetProperty(ref _userLogin, value))
                {
                    UpdateCommands();
                }
            }
        }
        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                if (SetProperty(ref _userPassword, value))
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

        public async void LoginAsync()
        {
            StartWorkIndication(ViewModelConstants.LOGIN_MESSAGE);
            
            await Task.Run(async () =>
            {
                var requestStatus = await _clientService.LoginWithCredentialsAsync(_userLogin, _userPassword);
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
                () => !string.IsNullOrEmpty(UserPassword) && !string.IsNullOrEmpty(UserLogin));
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

        private void LoadAuthenticationInfo()
        {
            var loadTask = Task.Run(async () => await _clientService.LoadAuthenticationInfoAsync());
            loadTask.ConfigureAwait(true);
            loadTask.Wait();
        }

        #endregion

        #region Fields

        private readonly IClientService _clientService;
        private readonly IPageNavigator _pageNavigator;

        private string _loginErrorMessage;
        private string _userPassword = "";
        private string _userLogin = "";

        #endregion
    }
}
