using My9GAG.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace My9GAG.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CommentsPage : ContentPage
	{
		public CommentsPage(CommentsPageViewModel viewModel)
		{
			InitializeComponent ();
            BindingContext = viewModel;
		}
	}
}
