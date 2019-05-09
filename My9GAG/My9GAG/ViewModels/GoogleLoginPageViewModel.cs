using My9GAG.Logic;
using My9GAG.Models.Request;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace My9GAG.ViewModels
{
    public class GoogleLoginPageViewModel : ViewModelBase
    {
        #region Constructors

        public GoogleLoginPageViewModel(
            IGoogleAuthenticationService googleAuthenticationService, 
            IClientService clientService, 
            IPageNavigator pageNavigator)
        {
            _googleAuthenticationService = googleAuthenticationService;
            _clientService = clientService;
            _pageNavigator = pageNavigator;

            InitCommands();

            PageUrl = _googleAuthenticationService.GetAuthenticationPageUrl();
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

        public async void LoginAsync(string redirectUrl)
        {
            StartWorkIndication(ViewModelConstants.LOGIN_MESSAGE);

            string code = RequestUtils.ExtractValueFromUrl(redirectUrl, CODE_URL_KEY);

            if (!string.IsNullOrWhiteSpace(code))
            {
                await Task.Run(async () =>
                {
                    string accessToken = await _googleAuthenticationService.GetAccessTokenAsync(code);

                    Debug.WriteLine("ACCESS TOKEN: " + accessToken);

                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        var requestStatus = await _clientService.LoginWithGoogleAsync(accessToken);
                        Debug.WriteLine(requestStatus.IsSuccessful + " " + requestStatus.Message);
                        if (requestStatus.IsSuccessful)
                        {
                            await Task.Delay(ViewModelConstants.LOGIN_DELAY);

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                _pageNavigator.OpenPostsPage();
                            });
                        }
                        else
                        {
                            //LoginErrorMessage = requestStatus.Message;
                        }
                    }
                });
            }

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
            NavigatingCommand = new Command(
                redirectUrl => LoginAsync((string)redirectUrl));
        }

        #endregion

        #region Fields

        private IGoogleAuthenticationService _googleAuthenticationService;
        private IClientService _clientService;
        private IPageNavigator _pageNavigator;
        private string _pageUrl;

        #endregion

        #region Constants

        private const string CODE_URL_KEY = "code";

        #endregion
    }
}
