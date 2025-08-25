using c971_project.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace c971_project
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            var homePage = serviceProvider.GetRequiredService<HomePage>();

            // Create ONE NavigationPage and use it
            var navigationPage = new NavigationPage(homePage);

            // Set navigation bar colors
            navigationPage.BarBackgroundColor = Color.FromArgb("#22223B"); // darkblue
            navigationPage.BarTextColor = Color.FromArgb("#F2E9E4"); // cream

            // Assign the styled navigation page
            MainPage = navigationPage;
        }


    }
}