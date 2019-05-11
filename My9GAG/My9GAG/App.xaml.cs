using Autofac;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using My9GAG.Logic.Client;
using My9GAG.Logic.FacebookAuthentication;
using My9GAG.Logic.GoogleAuthentication;
using My9GAG.Logic.PageNavigator;
using My9GAG.ViewModels;
using My9GAG.Views;
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

        #endregion

        #region Implementation

        protected override void OnStart()
        {
            string uwpSecret = "uwp=f606d5fa-abee-4270-8cb3-666339fcac43;";
            string androidSecret = "android=1a97b6b9-0417-46f5-bddf-e62439a394e7;";
            string iosSecret = "ios=aee2b868-4650-4d89-a6e2-bf7503dd97e8;";

            AppCenter.Start(uwpSecret + androidSecret + iosSecret, typeof(Analytics), typeof(Crashes));
        }
        protected override void OnSleep()
        {
            
        }
        protected override void OnResume()
        {

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
                OnOpenLoginWithFacebookPage = OpenLoginWithFacebookPage
            }).As<IPageNavigator>().SingleInstance();

            builder.RegisterType<LoginPageViewModel>();
            builder.RegisterType<LoginWithGooglePageViewModel>();
            builder.RegisterType<LoginWithFacebookPageViewModel>();
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

            bool isUserLoggedIn = false;

            if (Current.Properties.ContainsKey(USER_LOGGED_IN_KEY))
            {
                isUserLoggedIn = (bool)Current.Properties[USER_LOGGED_IN_KEY];
            }

            if (isUserLoggedIn)
            {
                OpenPostsPage();                
            }
            else
            {
                OpenLoginPage();
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
