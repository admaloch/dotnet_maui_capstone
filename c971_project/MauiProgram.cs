using c971_project.Services.Data;
using c971_project.Services.Notifications;
using c971_project.Services.ValidationServices;
using c971_project.ViewModels;
using c971_project.Views;
using c971_project.Services.Firebase;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;


namespace c971_project
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<CourseValidator>();
            builder.Services.AddSingleton<IScheduleNotificationService, NotificationService>();

            builder.Services.AddSingleton<AuthService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<HomeViewModel>();

            builder.Services.AddTransient<EditStudentPage>();
            builder.Services.AddTransient<EditStudentViewModel>();

            builder.Services.AddTransient<AddTermPage>();
            builder.Services.AddTransient<AddTermViewModel>();

            builder.Services.AddTransient<TermPage>();
            builder.Services.AddTransient<TermViewModel>();

            builder.Services.AddTransient<EditTermPage>();
            builder.Services.AddTransient<EditTermViewModel>();

            builder.Services.AddTransient<AddCoursePage>();
            builder.Services.AddTransient<AddCourseViewModel>();

            builder.Services.AddTransient<CoursePage>();
            builder.Services.AddTransient<CourseViewModel>();

            builder.Services.AddTransient<EditCoursePage>();
            builder.Services.AddTransient<EditCourseViewModel>();

            builder.Services.AddTransient<AddAssessmentPage>();
            builder.Services.AddTransient<AddAssessmentViewModel>();

            builder.Services.AddTransient<EditAssessmentPage>();
            builder.Services.AddTransient<EditAssessmentViewModel>();

            builder.Services.AddTransient<AddNotePage>();
            builder.Services.AddTransient<AddNoteViewModel>();

            builder.Services.AddTransient<NotePage>();
            builder.Services.AddTransient<NoteViewModel>();

            builder.Services.AddTransient<EditNotePage>();
            builder.Services.AddTransient<EditNoteViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}