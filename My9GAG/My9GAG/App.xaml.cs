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

            viewModel = new PostsPageViewModel();
            viewModel.RestoreState(Current.Properties);
            viewModel.OnOpenCommentsPage += OpenCommentsPage;
            MainPage = new NavigationPage(new My9GAG.Views.PostsPage(viewModel))
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
            // Handle when your app starts
        }
        protected override void OnSleep()
        {
            viewModel.SaveState(Current.Properties);
        }
        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private PostsPageViewModel viewModel;
    }
}
