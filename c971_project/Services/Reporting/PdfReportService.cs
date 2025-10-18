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

        public PdfReportService(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;
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
                                    // FIXED: 4 columns for 4 data fields
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(80); // Number
                                        columns.RelativeColumn(2);  // Name
                                        columns.RelativeColumn();   // Dates
                                        columns.ConstantColumn(60); // Credits
                                    });

                                    // FIXED: 4 header cells for 4 columns
                                    table.Header(header =>
                                    {
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Number");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Course Name");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Dates");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Credits");
                                    });

                                    // FIXED: 4 data cells for 4 columns
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
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Term_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .AlignCenter()
                        .Text("Term Overview Report")
                        .SemiBold().FontSize(20).FontColor(QuestPDFColors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(15);
                            column.Item().Text($"Generated: {reportData.GeneratedAt:MMMM dd, yyyy hh:mm tt}");
                            column.Item().Text($"Total Terms: {reportData.Terms.Count}");

                            if (reportData.Terms.Any())
                            {
                                foreach (var term in reportData.Terms)
                                {
                                    var termCourses = reportData.Courses.Where(c => c.TermId == term.Id).ToList();

                                    column.Item().Background(QuestPDFColors.Grey.Lighten2).Padding(15).Column(termColumn =>
                                    {
                                        // Term Header
                                        termColumn.Item().Text(term.Name).SemiBold().FontSize(16);
                                        termColumn.Item().Text($"{term.StartDate:MMM dd, yyyy} - {term.EndDate:MMM dd, yyyy}")
                                            .FontColor(QuestPDFColors.Grey.Medium);
                                        termColumn.Item().Text($"Courses: {termCourses.Count}")
                                            .FontColor(QuestPDFColors.Grey.Medium);

                                        // Courses Table
                                        if (termCourses.Any())
                                        {
                                            termColumn.Item().PaddingTop(10).Table(courseTable =>
                                            {
                                                // Updated columns based on your actual Course model
                                                courseTable.ColumnsDefinition(columns =>
                                                {
                                                    columns.RelativeColumn(2);  // Course Name
                                                    columns.RelativeColumn(1);  // Course Number
                                                    columns.RelativeColumn(1);  // Credits
                                                    columns.RelativeColumn(1);  // Dates
                                                });

                                                // Table Header
                                                courseTable.Header(header =>
                                                {
                                                    header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Course Name");
                                                    header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Course #");
                                                    header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Credits");
                                                    header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Dates");
                                                });

                                                // Table Rows - using actual Course properties
                                                foreach (var course in termCourses)
                                                {
                                                    courseTable.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(course.Name);
                                                    courseTable.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(course.CourseNum);
                                                    courseTable.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(course.CuNum.ToString());
                                                    courseTable.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text($"{course.StartDate:MM/dd} - {course.EndDate:MM/dd}");
                                                }
                                            });
                                        }
                                        else
                                        {
                                            termColumn.Item().PaddingTop(5).Text("No courses in this term").Italic().FontColor(QuestPDFColors.Grey.Medium);
                                        }
                                    });

                                    column.Item().Height(10); // Add some space between terms
                                }
                            }
                            else
                            {
                                column.Item().Text("No terms found.").Italic();
                            }
                        });
                });
            });

            document.GeneratePdf(filePath);
            return filePath;
        }

        public async Task<string> GenerateInstructorReportAsync(string userId)
        {
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Instructor_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .AlignCenter()
                        .Text("Instructor Contact Report")
                        .SemiBold().FontSize(20).FontColor(QuestPDFColors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(10);
                            column.Item().Text($"Generated: {reportData.GeneratedAt:MMMM dd, yyyy hh:mm tt}");
                            column.Item().Text($"Total Instructors: {reportData.Instructors.Count}");

                            if (reportData.Instructors.Any())
                            {
                                column.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(2);  // Name
                                        columns.RelativeColumn(2);  // Email
                                        columns.RelativeColumn();   // Phone
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Instructor");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Email");
                                        header.Cell().Background(QuestPDFColors.Grey.Lighten3).Padding(5).Text("Phone");
                                    });

                                    foreach (var instructor in reportData.Instructors)
                                    {
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(instructor.Name);
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(instructor.Email ?? "N/A");
                                        table.Cell().BorderBottom(1).BorderColor(QuestPDFColors.Grey.Lighten2).Padding(5).Text(instructor.Phone ?? "N/A");
                                    }
                                });
                            }
                            else
                            {
                                column.Item().Text("No instructors found.").Italic();
                            }
                        });
                });
            });

            document.GeneratePdf(filePath);
            return filePath;
        }

        public async Task<string> GenerateComprehensiveReportAsync(string userId)
        {
            var reportData = await LoadReportDataAsync(userId);
            var fileName = $"Comprehensive_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);

                    page.Header()
                        .AlignCenter()
                        .Text("Comprehensive Academic Report")
                        .SemiBold().FontSize(20).FontColor(QuestPDFColors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            // Summary
                            column.Item().Background(QuestPDFColors.Grey.Lighten3).Padding(10).Column(summaryColumn =>
                            {
                                summaryColumn.Item().Text("Summary").SemiBold().FontSize(16);
                                summaryColumn.Item().Text($"Generated: {reportData.GeneratedAt:MMMM dd, yyyy hh:mm tt}");
                                summaryColumn.Item().Text($"Terms: {reportData.Terms.Count}");
                                summaryColumn.Item().Text($"Courses: {reportData.Courses.Count}");
                                summaryColumn.Item().Text($"Assessments: {reportData.Assessments.Count}");
                                summaryColumn.Item().Text($"Instructors: {reportData.Instructors.Count}");
                            });

                            // Recent Assessments
                            var recentAssessments = reportData.Assessments
                                .Where(a => a.EndDate >= DateTime.Today)
                                .OrderBy(a => a.EndDate)
                                .Take(5)
                                .ToList();

                            if (recentAssessments.Any())
                            {
                                column.Item().Text("Upcoming Assessments").SemiBold().FontSize(14);
                                foreach (var assessment in recentAssessments)
                                {
                                    var course = reportData.Courses.FirstOrDefault(c => c.Id == assessment.CourseId);
                                    column.Item().Padding(5).Text($"{assessment.Name} - Due: {assessment.EndDate:MMM dd} ({course?.Name ?? "Unknown Course"})");
                                }
                            }
                        });
                });
            });

            document.GeneratePdf(filePath);
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