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

            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<EditStudentPage>();
            builder.Services.AddTransient<StudentViewModel>();
            builder.Services.AddTransient<AddTermViewModel>();
            builder.Services.AddTransient<AddTermPage>();
            builder.Services.AddTransient<TermPage>();
            builder.Services.AddTransient<TermViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}