using c971_project.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace c971_project
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Set MainPage to the AppShell
            MainPage = new AppShell();
            //MainPage = serviceProvider.GetRequiredService<AppShell>();


            //RequestNotificationPermission();

        }
        private async void RequestNotificationPermission()
        {
            try
            {
                // Use LocalNotificationCenter.Current, not NotificationCenter
                var permission = await Plugin.LocalNotification.LocalNotificationCenter.Current.RequestNotificationPermission();
                Debug.WriteLine($"App startup permission: {permission}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Permission request error: {ex.Message}");
            }
        }
    }
}
