using c971_project.ViewModels;

namespace c971_project.Views
{
    public partial class EditTermPage : ContentPage
    {
        public EditTermPage(EditTermViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
