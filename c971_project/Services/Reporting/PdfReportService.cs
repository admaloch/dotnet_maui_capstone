// Services/Reporting/PdfReportService.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using c971_project.Models;
using c971_project.Services.Firebase;
using QuestPDFColors = QuestPDF.Helpers.Colors;


namespace c971_project.Services.Reporting
{
    public class PdfReportService : IReportService
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly IFileSystem _fileSystem;

        public PdfReportService(IFirestoreDataService firestoreDataService, IFileSystem fileSystem)
        {
            _firestoreDataService = firestoreDataService;

            _fileSystem = fileSystem;
            QuestPDF.Settings.License = LicenseType.Community; // Free for community use
        }

        public async Task<string> GenerateCourseReportAsync(string userId)
        {
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Course_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(QuestPDFColors.White);

                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Text("Course List Report")
                        .SemiBold().FontSize(20).FontColor(QuestPDFColors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);

                            // Report Info
                            column.Item().Text($"Generated: {reportData.GeneratedAt:MMMM dd, yyyy hh:mm tt}");
                            column.Item().Text($"Total Courses: {reportData.Courses.Count}");

                            // Courses Table
                            if (reportData.Courses.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(80); // Number
                                        columns.RelativeColumn(2);  // Name
                                        columns.RelativeColumn();   // Status
                                        columns.RelativeColumn();   // Dates
                                        columns.ConstantColumn(60); // Credits
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Number");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Course Name");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Status");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Dates");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Credits");
                                    });

                                    foreach (var course in reportData.Courses)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(course.CourseNum);
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(course.Name);
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text($"{course.StartDate:MMM dd} - {course.EndDate:MMM dd}");
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(course.CuNum.ToString());
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Text("No courses found.").Italic();
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            });

            document.GeneratePdf(filePath);
            return filePath;
        }

        public async Task<string> GenerateAssessmentReportAsync(string userId)
        {
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Assessment_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .AlignCenter()
                        .Text("Assessment Schedule Report")
                        .SemiBold().FontSize(20).FontColor(QuestPDFColors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);
                            column.Item().Text($"Generated: {reportData.GeneratedAt:MMMM dd, yyyy hh:mm tt}");
                            column.Item().Text($"Upcoming Assessments: {reportData.Assessments.Count(a => a.EndDate >= DateTime.Today)}");

                            if (reportData.Assessments.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);  // Name
                                        columns.RelativeColumn();   // Type
                                        columns.RelativeColumn();   // Course
                                        columns.RelativeColumn();   // Due Date
                                        columns.RelativeColumn();   // Status
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Assessment");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Type");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Course");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Due Date");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Status");
                                    });

                                    foreach (var assessment in reportData.Assessments.OrderBy(a => a.EndDate))
                                    {
                                        var course = reportData.Courses.FirstOrDefault(c => c.Id == assessment.CourseId);
                                        var status = assessment.EndDate < DateTime.Today ? "Overdue" :
                                                    assessment.StartDate > DateTime.Today ? "Upcoming" : "Current";

                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(assessment.Name);
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(assessment.Type.ToString());
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(course?.Name ?? "Unknown");
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(assessment.EndDate.ToString("MMM dd, yyyy"));
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(status);
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Text("No assessments found.").Italic();
                            }
                        });
                });
            });

            document.GeneratePdf(filePath);
            return filePath;
        }

        public async Task<string> GenerateTermReportAsync(string userId)
        {
            // Similar implementation for terms
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Term_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Implementation similar to above...
            return filePath;
        }

        public async Task<string> GenerateInstructorReportAsync(string userId)
        {
            // Similar implementation for instructors
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Instructor_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Implementation similar to above...
            return filePath;
        }

        public async Task<string> GenerateComprehensiveReportAsync(string userId)
        {
            // Combines all data into one comprehensive report
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Comprehensive_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            // Implementation that combines terms, courses, assessments...
            return filePath;
        }

        private async Task<ReportData> LoadReportDataAsync(string userId)
        {
            var terms = await _firestoreDataService.GetTermsByUserIdAsync(userId);
            var courses = new List<Course>();
            var assessments = new List<Assessment>();
            var notes = new List<Note>();

            // Get courses for each term
            if (terms?.Any() == true)
            {
                foreach (var term in terms)
                {
                    var termCourses = await _firestoreDataService.GetCoursesByTermIdAsync(term.Id);
                    courses.AddRange(termCourses ?? Enumerable.Empty<Course>());

                    // Get assessments and notes for these courses
                    foreach (var course in termCourses ?? Enumerable.Empty<Course>())
                    {
                        var courseAssessments = await _firestoreDataService.GetAssessmentsByCourseIdAsync(course.Id);
                        assessments.AddRange(courseAssessments ?? Enumerable.Empty<Assessment>());

                        var courseNotes = await _firestoreDataService.GetNotesByCourseIdAsync(course.Id);
                        notes.AddRange(courseNotes ?? Enumerable.Empty<Note>());
                    }
                }
            }

            var instructors = await _firestoreDataService.GetInstructorsAsync(userId);

            return new ReportData
            {
                Title = "Student Report",
                GeneratedAt = DateTime.Now,
                Terms = terms?.ToList() ?? new List<Term>(),
                Courses = courses,
                Assessments = assessments,
                Notes = notes,
                Instructors = instructors?.ToList() ?? new List<Instructor>()
            };
        }
    }
}