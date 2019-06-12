using Autofac;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using My9GAG.Logic.Client;
using My9GAG.Logic.FacebookAuthentication;
using My9GAG.Logic.GoogleAuthentication;
using My9GAG.Logic.PageNavigator;
using My9GAG.Models.User;
using My9GAG.ViewModels;
using My9GAG.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace My9GAG
{
    public partial class App : Application
	{
        #region Constructors

        public App()
        {
            InitializeComponent();
            RegisterContainer();
            SetupPages();
        }

        #endregion

        #region Methods

        public async void GoBack()
        {
            if (MainPage.Navigation.NavigationStack.Count > 1)
            {
                await MainPage.Navigation.PopAsync();
            }
        }
        public async void OpenPostsPage()
        {
            var postsPage = new PostsPage()
            {
                BindingContext = _container.Resolve<PostsPageViewModel>()
            };
            NavigationPage.SetHasBackButton(postsPage, false);
            MainPage.Navigation.InsertPageBefore(postsPage, MainPage.Navigation.NavigationStack[0]);
            await MainPage.Navigation.PopToRootAsync();
        }
        public async void OpenCommentsPage()
        {
            var commentsPage = new CommentsPage()
            {
                BindingContext = _container.Resolve<CommentsPageViewModel>()
            };
            NavigationPage.SetHasBackButton(commentsPage, true);
            await MainPage.Navigation.PushAsync(commentsPage);
        }
        public async void OpenLoginPage()
        {
            var loginPage = new LoginPage()
            {
                BindingContext = _container.Resolve<LoginPageViewModel>()
            };
            NavigationPage.SetHasBackButton(loginPage, true);
            await MainPage.Navigation.PushAsync(loginPage);
        }
        public async void OpenLoginWithGooglePage()
        {
            var googleLoginPage = new LoginWithGooglePage()
            {
                BindingContext = _container.Resolve<LoginWithGooglePageViewModel>()
            };
            NavigationPage.SetHasBackButton(googleLoginPage, true);
            await MainPage.Navigation.PushAsync(googleLoginPage);
        }
        public async void OpenLoginWithFacebookPage()
        {
            var facebookLoginPage = new LoginWithFacebookPage()
            {
                BindingContext = _container.Resolve<LoginWithFacebookPageViewModel>()
            };
            NavigationPage.SetHasBackButton(facebookLoginPage, true);
            await MainPage.Navigation.PushAsync(facebookLoginPage);
        }
        public async void OpenRegistrationPage()
        {
            var registrationPage = new RegistrationPage()
            {
                BindingContext = _container.Resolve<RegistrationPageViewModel>()
            };
            NavigationPage.SetHasBackButton(registrationPage, true);
            await MainPage.Navigation.PushAsync(registrationPage);
        }

        #endregion

        #region Implementation

        protected override void OnStart()
        {
            string uwpSecret = "uwp=f606d5fa-abee-4270-8cb3-666339fcac43;";
            string androidSecret = "android=1a97b6b9-0417-46f5-bddf-e62439a394e7;";
            string iosSecret = "ios=aee2b868-4650-4d89-a6e2-bf7503dd97e8;";

            AppCenter.Start(uwpSecret + androidSecret + iosSecret, typeof(Analytics), typeof(Crashes));

            if (_tokenExpirationCheckLoop == null)
            {
                _tokenExpirationCheckLoop = StartTokenExpirationCheckLoop();
            }
        }
        protected override void OnSleep()
        {
            var clientService = _container.Resolve<IClientService>();
            clientService.SaveState(Current.Properties);

            if (_tokenExpirationCheckLoop != null)
            {
                _tokenExpirationCheckLoop.Cancel();
            }
        }
        protected override void OnResume()
        {
            if (_tokenExpirationCheckLoop == null)
            {
                _tokenExpirationCheckLoop = StartTokenExpirationCheckLoop();
            }
        }

        private void RegisterContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<GoogleAuthenticationService>().As<IGoogleAuthenticationService>();
            builder.RegisterType<FacebookAuthenticationService>().As<IFacebookAuthenticationService>();
            builder.RegisterType<ClientService>().As<IClientService>().SingleInstance();
            builder.Register(navigator => new PageNavigator()
            {
                OnGoBack = GoBack,
                OnOpenPostsPage = OpenPostsPage,
                OnOpenCommentsPage = OpenCommentsPage,
                OnOpenLoginPage = OpenLoginPage,
                OnOpenLoginWithGooglePage = OpenLoginWithGooglePage,
                OnOpenLoginWithFacebookPage = OpenLoginWithFacebookPage,
                OnOpenRegistrationPage = OpenRegistrationPage
            }).As<IPageNavigator>().SingleInstance();

            builder.RegisterType<LoginPageViewModel>();
            builder.RegisterType<LoginWithGooglePageViewModel>();
            builder.RegisterType<LoginWithFacebookPageViewModel>();
            builder.RegisterType<RegistrationPageViewModel>();
            builder.RegisterType<PostsPageViewModel>();
            builder.RegisterType<CommentsPageViewModel>();

            _container = builder.Build();
        }
        private void SetupPages()
        {
            MainPage = new NavigationPage()
            {
                BarBackgroundColor = Color.Black,
                BarTextColor = Color.White,
                BackgroundColor = Color.Black,
                Title = APP_NAME
            };

            var clientService = _container.Resolve<IClientService>();
            clientService.RestoreState(Current.Properties);
            var user = clientService.User;

            bool userNeedsLogIn = user.LoginStatus == LoginStatus.None ||
                (user.TokenExpirationTime - DateTime.UtcNow).TotalMinutes < 30;

            if (userNeedsLogIn)
            {
                OpenLoginPage();
            }
            else
            {
                OpenPostsPage();
            }
        }
        private CancellationTokenSource StartTokenExpirationCheckLoop()
        {
            var clientService = _container.Resolve<IClientService>();
            var navigationStack = MainPage.Navigation.NavigationStack;
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            Task.Run(async () =>
            {
                while (true)
                {
                    if (clientService.User.LoginStatus != LoginStatus.None && clientService.User.TokenExpired() && navigationStack.Count > 0)
                    {
                        if (navigationStack[navigationStack.Count - 1].BindingContext is ViewModelBase viewModel)
                        {
                            //await viewModel.AutoLoginAsync();
                        }
                    }

                    await Task.Delay(CHECK_TOKEN_EXPIRATION_DELAY);
                }
            }, cancellationToken);

            return cancellationTokenSource;
        }
        private void StopTokenExpirationCheckLoop()
        {

        }

        #endregion

        #region Fields

        private IContainer _container;
        private CancellationTokenSource _tokenExpirationCheckLoop;

        #endregion

        #region Constants

        private const string APP_NAME = "My9GAG";
        private const string USER_LOGGED_IN_KEY = "isUserLoggedIn";
        private const int CHECK_TOKEN_EXPIRATION_DELAY = 30000;

        #endregion
    }
}
