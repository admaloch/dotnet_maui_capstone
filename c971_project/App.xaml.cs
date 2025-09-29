using System.Diagnostics;


using c971_project.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using c971_project.Services.Data;

namespace c971_project
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Show loading page immediately
            MainPage = new ContentPage
            {
                Content = new Label
                {
                    Text = "Loading...",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };

            // Start initialization in background (fire and forget)
            _ = InitializeAppAsync(serviceProvider);
        }

        private async Task InitializeAppAsync(IServiceProvider serviceProvider)
        {
            try
            {
                // Get the database service and wait for it to initialize
                var dbService = serviceProvider.GetRequiredService<DatabaseService>();
                await dbService.EnsureInitialized(); // Properly wait for DB to be ready

                // Switch to main app
                MainPage = new AppShell();
            }
            catch (Exception ex)
            {
                // Handle any initialization errors gracefully
                MainPage = new ContentPage
                {
                    Content = new Label
                    {
                        Text = $"Initialization Error: {ex.Message}",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                };
                Debug.WriteLine($"App initialization failed: {ex}");
            }
        }
    }
}
