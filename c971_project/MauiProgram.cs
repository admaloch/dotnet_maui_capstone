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

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<HomeViewModel>();

            builder.Services.AddTransient<EditStudentPage>();
            builder.Services.AddTransient<StudentViewModel>();

            builder.Services.AddTransient<AddTermPage>();
            builder.Services.AddTransient<AddTermViewModel>();

            builder.Services.AddTransient<TermPage>();
            builder.Services.AddTransient<TermViewModel>();

            builder.Services.AddTransient<AddCoursePage>();
            builder.Services.AddTransient<AddCourseViewModel>();

            builder.Services.AddTransient<CoursePage>();
            builder.Services.AddTransient<CourseViewModel>();


            builder.Services.AddTransient<EditCoursePage>();
            builder.Services.AddTransient<EditCourseViewModel>();

            builder.Services.AddTransient<AddAssessmentPage>();
            builder.Services.AddTransient<AddAssessmentViewModel>();

            builder.Services.AddTransient<AddNotePage>();
            builder.Services.AddTransient<AddNoteViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}