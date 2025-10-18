using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class ReportsPage : ContentPage
    {
        public ReportsPage(ReportsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}