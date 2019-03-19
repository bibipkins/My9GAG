using My9GAG.ViewModels;
using Xamarin.Forms;

namespace My9GAG.Views
{
    public partial class PostsPage : ContentPage
	{
		public PostsPage(PostsPageViewModel viewModel)
		{
			InitializeComponent();
            BindingContext = viewModel;
        }
    }
}