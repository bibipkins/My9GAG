using Autofac;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using My9GAG.Logic.Client;
using My9GAG.Logic.FacebookAuthentication;
using My9GAG.Logic.GoogleAuthentication;
using My9GAG.Logic.Logger;
using My9GAG.Logic.PageNavigator;
using My9GAG.Logic.SecureStorage;
using My9GAG.ViewModels;
using My9GAG.Views;
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

        #region Lifecycle

        protected override void OnStart()
        {
            string uwpSecret = "uwp=f606d5fa-abee-4270-8cb3-666339fcac43;";
            string androidSecret = "android=1a97b6b9-0417-46f5-bddf-e62439a394e7;";
            string iosSecret = "ios=aee2b868-4650-4d89-a6e2-bf7503dd97e8;";

            AppCenter.Start(uwpSecret + androidSecret + iosSecret, typeof(Analytics), typeof(Crashes));
        }
        protected override void OnSleep()
        {
            var clientService = _container.Resolve<IClientService>();
            clientService.SaveState(Current.Properties);
        }
        protected override void OnResume()
        {

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
        public async void GoToPostsPage(PostsPageViewModel viewModel, bool canGoBack)
        {
            await GoToPage(new PostsPage()
            {
                BindingContext = viewModel ?? _container.Resolve<PostsPageViewModel>()
            }, canGoBack);
        }
        public async void GoToCommentsPage(CommentsPageViewModel viewModel, bool canGoBack)
        {
            await GoToPage(new CommentsPage()
            {
                BindingContext = viewModel ?? _container.Resolve<CommentsPageViewModel>()
            }, canGoBack);
        }
        public async void GoToLoginPage(LoginPageViewModel viewModel, bool canGoBack)
        {
            await GoToPage(new LoginPage()
            {
                BindingContext = viewModel ?? _container.Resolve<LoginPageViewModel>()
            }, canGoBack);
        }
        public async void GoToLoginWithGooglePage(LoginWithGooglePageViewModel viewModel, bool canGoBack)
        {
            await GoToPage(new LoginWithGooglePage()
            {
                BindingContext = viewModel ?? _container.Resolve<LoginWithGooglePageViewModel>()
            }, canGoBack);
        }
        public async void GoToLoginWithFacebookPage(LoginWithFacebookPageViewModel viewModel, bool canGoBack)
        {
            await GoToPage(new LoginWithFacebookPage()
            {
                BindingContext = viewModel ?? _container.Resolve<LoginWithFacebookPageViewModel>()
            }, canGoBack);
        }
        public async void GoToRegistrationPage(RegistrationPageViewModel viewModel, bool canGoBack)
        {
            await GoToPage(new RegistrationPage()
            {
                BindingContext = viewModel ?? _container.Resolve<RegistrationPageViewModel>()
            }, canGoBack);
        }

        #endregion

        #region Implementation

        private void RegisterContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleLogger>().As<ILogger>();
            builder.RegisterType<GoogleAuthenticationService>().As<IGoogleAuthenticationService>();
            builder.RegisterType<FacebookAuthenticationService>().As<IFacebookAuthenticationService>();
            builder.RegisterType<SecureStorage>().As<ISecureStorage>();
            builder.RegisterType<ClientService>().As<IClientService>().SingleInstance();
            builder.Register(navigator => new PageNavigator()
            {
                OnGoBack = GoBack,
                OnOpenPostsPage = GoToPostsPage,
                OnOpenCommentsPage = GoToCommentsPage,
                OnOpenLoginPage = GoToLoginPage,
                OnOpenLoginWithGooglePage = GoToLoginWithGooglePage,
                OnOpenLoginWithFacebookPage = GoToLoginWithFacebookPage,
                OnOpenRegistrationPage = GoToRegistrationPage
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

            MainPage.Navigation.PushAsync(new LoginPage()
            {
                BindingContext = _container.Resolve<LoginPageViewModel>()
            });

            var clientService = _container.Resolve<IClientService>();
            clientService.RestoreState(Current.Properties);
        }
        private async Task GoToPage(ContentPage page, bool canGoBack)
        {
            NavigationPage.SetHasBackButton(page, canGoBack);
            
            if (canGoBack)
            {
                await MainPage.Navigation.PushAsync(page);
            }
            else
            {
                MainPage.Navigation.InsertPageBefore(page, MainPage.Navigation.NavigationStack[0]);
                await MainPage.Navigation.PopToRootAsync();
            }
        }

        #endregion

        #region Fields

        private IContainer _container;

        #endregion

        #region Constants

        private const string APP_NAME = "My9GAG";
        private const string USER_LOGGED_IN_KEY = "isUserLoggedIn";

        #endregion
    }
}
