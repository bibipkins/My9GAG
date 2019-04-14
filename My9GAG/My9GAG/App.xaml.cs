using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using My9GAG.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace My9GAG
{
    public partial class App : Application
	{
        public App()
        {
            InitializeComponent();

            _viewModel = new PostsPageViewModel();
            _viewModel.RestoreState(Current.Properties);
            _viewModel.OnOpenCommentsPage += OpenCommentsPage;
            MainPage = new NavigationPage(new My9GAG.Views.PostsPage(_viewModel))
            {
                BarBackgroundColor = Color.Black,
                BarTextColor = Color.White,
                BackgroundColor = Color.Black,
                Title = "My9GAG"
            };
        }

        public void OpenCommentsPage(object sender, CommentsPageViewModel viewModel)
        {
            var commentsPage = new Views.CommentsPage(viewModel);
            NavigationPage.SetHasBackButton(commentsPage, true);
            MainPage.Navigation.PushAsync(commentsPage);
        }

        protected override void OnStart()
        {
            string uwpSecret = "uwp=f606d5fa-abee-4270-8cb3-666339fcac43;";
            string androidSecret = "android=1a97b6b9-0417-46f5-bddf-e62439a394e7;";
            string iosSecret = "ios=aee2b868-4650-4d89-a6e2-bf7503dd97e8;";

            AppCenter.Start(uwpSecret + androidSecret + iosSecret, typeof(Analytics), typeof(Crashes));
        }
        protected override void OnSleep()
        {
            _viewModel.SaveState(Current.Properties);
        }
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private PostsPageViewModel _viewModel;
    }
}
