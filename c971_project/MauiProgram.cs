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

            // Services - Singleton (shared resources)
            builder.Services.AddSingleton<DatabaseService>();

            // Pages - Transient (new instance each time)
            builder.Services.AddTransient<HomePage>();
            // Add these as you create them:
            // builder.Services.AddTransient<TermDetailPage>();
            // builder.Services.AddTransient<AddEditTermPage>(); 
            // builder.Services.AddTransient<CourseDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}