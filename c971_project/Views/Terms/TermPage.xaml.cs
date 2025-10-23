using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class TermPage : ContentPage
    {
        public TermPage(TermViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}