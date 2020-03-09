using My9GAG.Logic.Client;
using My9GAG.Logic.PageNavigator;
using NineGagApiClient.FacebookAuthentication;
using NineGagApiClient.Utils;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace My9GAG.ViewModels
{
    public class LoginWithFacebookPageViewModel : ViewModelBase
    {
        #region Constructors

        public LoginWithFacebookPageViewModel(
            IFacebookAuthenticationService facebookAuthenticationService,
            IClientService clientService,
            IPageNavigator pageNavigator)
        {
            _facebookAuthenticationService = facebookAuthenticationService;
            _clientService = clientService;
            _pageNavigator = pageNavigator;

            InitCommands();

            _state = RequestUtils.GetUuid();
            PageUrl = _facebookAuthenticationService.GetAuthenticationPageUrl(_state);
        }

        #endregion

        #region Properties

        public string PageUrl
        {
            get { return _pageUrl; }
            set { SetProperty(ref _pageUrl, value); }
        }

        #endregion

        #region Methods

        public async void LoginAsync(string token)
        {
            StartWorkIndication(ViewModelConstants.LOGIN_MESSAGE);

            await Task.Run(async () =>
            {
                await _clientService.LoginWithFacebookAsync(token);
                var requestStatus = await _clientService.LoginWithFacebookAsync(token);

                if (requestStatus.IsSuccessful)
                {
                    await Task.Delay(ViewModelConstants.LOGIN_DELAY);
                    _pageNavigator.GoToPostsPage(null, false);
                }
                else
                {
                    await ShowMessage(ViewModelConstants.LOGIN_FAILED_MESSAGE);
                    _pageNavigator.GoBack();
                }
            });

            StopWorkIndication();
        }

        #endregion

        #region Commands

        public ICommand NavigatingCommand
        {
            get;
            protected set;
        }

        #endregion

        #region Implementation

        private void InitCommands()
        {
            NavigatingCommand = new Command<WebNavigatingEventArgs>(async e =>
            {
                string token = RequestUtils.ExtractValueFromUrl(e.Url, URL_ACCESS_TOKEN_ATTRIBUTE_KEY);
                string state = RequestUtils.ExtractValueFromUrl(e.Url, URL_STATE_ATTRIBUTE_KEY);
                string error = RequestUtils.ExtractValueFromUrl(e.Url, URL_ERROR_ATTRIBUTE_KEY);

                if (!string.IsNullOrWhiteSpace(token) && _state == state)
                {
                    e.Cancel = true;
                    LoginAsync(token);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(error))
                {
                    e.Cancel = true;
                    await ShowMessage(ViewModelConstants.LOGIN_WITH_FACEBOOK_FAILED_MESSAGE);
                    _pageNavigator.GoBack();
                }
            });
        }

        #endregion

        #region Fields

        private readonly IFacebookAuthenticationService _facebookAuthenticationService;
        private readonly IClientService _clientService;
        private readonly IPageNavigator _pageNavigator;

        private string _pageUrl;
        private string _state;

        #endregion

        #region Constants

        private const string URL_ACCESS_TOKEN_ATTRIBUTE_KEY = "access_token";
        private const string URL_STATE_ATTRIBUTE_KEY = "state";
        private const string URL_ERROR_ATTRIBUTE_KEY = "error";

        #endregion
    }
}
