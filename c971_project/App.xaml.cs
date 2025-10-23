using c971_project.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace c971_project
{
    public partial class App : Application
    {
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // SIMPLE: Let AppShell handle everything through DI
            MainPage = serviceProvider.GetService<AppShell>();
        }
    }
}