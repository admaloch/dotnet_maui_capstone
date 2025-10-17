using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class SearchPage : ContentPage
    {
        public SearchPage(SearchViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}