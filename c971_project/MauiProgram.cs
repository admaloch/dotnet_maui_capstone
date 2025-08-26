using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using c971_project.Services;
using c971_project.Views;
using c971_project.ViewModels;

namespace c971_project
{
    public static class MauiProgram
    {
        // Initialize with default value to avoid null warnings
        public static IServiceProvider Services { get; private set; } = null!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<DatabaseService>();

            // Register ViewModels
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<StudentViewModel>();

            // Register Pages
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<EditStudentPage>();

            // Add debug logging
            builder.Services.AddLogging(configure =>
            {
                configure.AddDebug();
            });

            var app = builder.Build();
            Services = app.Services;
            return app;
        }
    }
}