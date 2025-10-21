using c971_project.Services.Firebase;
using c971_project.Services.Notifications;
using c971_project.Services.Reporting;
using c971_project.Services.Search;
using c971_project.Services.ValidationServices;
using c971_project.ViewModels;
using c971_project.Views;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using QuestPDF.Infrastructure;


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

            QuestPDF.Settings.License = LicenseType.Community;

            builder.Services.AddSingleton<AppShell>();

            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddSingleton<IFirestoreDataService, FirestoreDataService>();

            builder.Services.AddSingleton<CourseValidator>();
            builder.Services.AddSingleton<IScheduleNotificationService, NotificationService>();
            builder.Services.AddSingleton<SearchService>();
            builder.Services.AddSingleton<IReportService, PdfReportService>();

            // view models
            builder.Services.AddTransient<ReportsViewModel>();
            builder.Services.AddTransient<ReportsPage>();

            builder.Services.AddTransient<SearchViewModel>();
            builder.Services.AddTransient<SearchPage>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();

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

            builder.Services.AddTransient<AssessmentPage>();
            builder.Services.AddTransient<AssessmentViewModel>();
            builder.Services.AddTransient<AddAssessmentPage>();
            builder.Services.AddTransient<AddAssessmentViewModel>();
            builder.Services.AddTransient<EditAssessmentPage>();
            builder.Services.AddTransient<EditAssessmentViewModel>();

            builder.Services.AddTransient<NotePage>();
            builder.Services.AddTransient<NoteViewModel>();
            builder.Services.AddTransient<AddNotePage>();
            builder.Services.AddTransient<AddNoteViewModel>();
            builder.Services.AddTransient<EditNotePage>();
            builder.Services.AddTransient<EditNoteViewModel>();

            builder.Services.AddTransient<InstructorPage>();
            builder.Services.AddTransient<InstructorViewModel>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}