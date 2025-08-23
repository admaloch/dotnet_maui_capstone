using c971_project.Views;

namespace c971_project
{
    public partial class App : Application
    {
        public App(HomePage homePage) // Injected -- HomePage is provided by the DI container
        {
            InitializeComponent();

            MainPage = new NavigationPage(homePage); // Set the main page to the injected HomePage -- wrapped in a NavigationPage for navigation support
        }

    }
}