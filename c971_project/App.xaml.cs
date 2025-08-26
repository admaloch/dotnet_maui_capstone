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

            // Set MainPage to the AppShell
            MainPage = serviceProvider.GetRequiredService<AppShell>();
        }
    }
}
