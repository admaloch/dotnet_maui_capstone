using Microsoft.Extensions.Logging;
using c971_project.Services;
using c971_project.Views;
using c971_project.ViewModels;

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

            // ViewModels - Transient (new instance for each page)
            builder.Services.AddTransient<StudentViewModel>();

            // Pages - Transient (new instance each time)
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<EditStudentPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}