// Services/Reporting/IReportService.cs
using c971_project.Models;

namespace c971_project.Services.Reporting
{
    public interface IReportService
    {
        Task<string> GenerateCourseReportAsync(string userId);
        Task<string> GenerateAssessmentReportAsync(string userId);
        Task<string> GenerateTermReportAsync(string userId);
        Task<string> GenerateInstructorReportAsync(string userId);
        Task<string> GenerateComprehensiveReportAsync(string userId);
    }

    public class ReportData
    {
        public string Title { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
        public List<Course> Courses { get; set; } = new();
        public List<Assessment> Assessments { get; set; } = new();
        public List<Term> Terms { get; set; } = new();
        public List<Instructor> Instructors { get; set; } = new();
        public List<Note> Notes { get; set; } = new();
    }
}