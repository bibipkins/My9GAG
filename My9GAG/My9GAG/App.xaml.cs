using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using My9GAG.Logic;
using My9GAG.ViewModels;
using My9GAG.Views;
using System.Diagnostics;
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

        public async void OpenPostsPage()
        {
            var postsPage = new PostsPage();
            NavigationPage.SetHasBackButton(postsPage, false);
            MainPage.Navigation.InsertPageBefore(postsPage, MainPage.Navigation.NavigationStack[0]);
            await MainPage.Navigation.PopToRootAsync();
        }
        public async void OpenCommentsPage()
        {
            var commentsPage = new CommentsPage();
            NavigationPage.SetHasBackButton(commentsPage, true);
            await MainPage.Navigation.PushAsync(commentsPage);
        }
        public async void OpenLoginPage()
        {
            var loginPage = new LoginPage();
            NavigationPage.SetHasBackButton(loginPage, true);
            await MainPage.Navigation.PushAsync(loginPage);
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
            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<IClientService, ClientService>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IPageNavigator, PageNavigator>(new ContainerControlledLifetimeManager(), 
                new InjectionFactory(navigator => new PageNavigator()
                {
                    OnOpenPostsPage = OpenPostsPage,
                    OnOpenCommentsPage = OpenCommentsPage,
                    OnOpenLoginPage = OpenLoginPage
                }));

            unityContainer.RegisterInstance(typeof(LoginPageViewModel));
            unityContainer.RegisterInstance(typeof(PostsPageViewModel));
            unityContainer.RegisterInstance(typeof(CommentsPageViewModel));

            _unityServiceLocator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => _unityServiceLocator);
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

        UnityServiceLocator _unityServiceLocator;

        #endregion

        #region Constants

        private const string APP_NAME = "My9GAG";
        private const string USER_LOGGED_IN_KEY = "isUserLoggedIn";

        #endregion
    }
}
