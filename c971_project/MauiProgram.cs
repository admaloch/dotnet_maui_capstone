using Microsoft.Extensions.Logging;
using c971_project.Services;
using c971_project.Views;

namespace c971_project
{
    public static class MauiProgram
    {
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

            // Register your services
            builder.Services.AddSingleton<DatabaseService>();// Singleton for shared database access -- one shared instance for the app

            // Register your pages
            builder.Services.AddTransient<HomePage>(); // Transient for pages -- create a new instance each time

#if DEBUG
            builder.Logging.AddDebug();
            #endif

            return builder.Build();
        }
    }
}