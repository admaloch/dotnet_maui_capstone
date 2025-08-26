using c971_project.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;

namespace c971_project
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Use the static Services property from MauiProgram
            var homePage = MauiProgram.Services.GetRequiredService<HomePage>();
            return new Window(new NavigationPage(homePage));
        }
    }
}