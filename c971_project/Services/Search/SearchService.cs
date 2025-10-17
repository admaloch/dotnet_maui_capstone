// Services/SearchService.cs
using System.Collections.ObjectModel;
using System.Linq;
using c971_project.Models;
using c971_project.Services.Firebase;

namespace c971_project.Services.Search
{
    public interface ISearchService
    {
        Task<SearchResults> SearchAsync(string query, string userId);
    }

    public class SearchService : ISearchService
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly AuthService _authService;


        public SearchService(IFirestoreDataService firestoreDataService, AuthService authService)
        {
            _firestoreDataService = firestoreDataService;
            _authService = authService;
        }

        public async Task<SearchResults> SearchAsync(string query, string userId)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new SearchResults();

            var lowerQuery = query.ToLower();
            var results = new SearchResults();

            try
            {
                // Search across all entities
                var terms = await _firestoreDataService.GetTermsByUserIdAsync(userId);
                var courses = await _firestoreDataService.GetCoursesByUserIdAsync(userId);
                var assessments = await _firestoreDataService.GetAssessmentsByUserIdAsync(userId);
                var notes = await _firestoreDataService.GetNotesByUserIdAsync(userId);
                var instructors = await _firestoreDataService.GetInstructorsAsync(userId); // Add this


                //  terms
                results.Terms = new ObservableCollection<Term>(
                    terms?.Where(t =>
                        t.Name?.ToLower().Contains(lowerQuery) == true ||
                        t.StartDate.ToString("MM/dd/yyyy").Contains(query) == true ||
                        t.EndDate.ToString("MM/dd/yyyy").Contains(query) == true
                    ) ?? Enumerable.Empty<Term>()
                );

                //  courses
                results.Courses = new ObservableCollection<Course>(
                    courses?.Where(c =>
                        c.Name?.ToLower().Contains(lowerQuery) == true ||
                        c.CourseNum?.ToLower().Contains(lowerQuery) == true ||
                        c.StartDate.ToString("MM/dd/yyyy").Contains(query) == true ||
                        c.EndDate.ToString("MM/dd/yyyy").Contains(query) == true
                    ) ?? Enumerable.Empty<Course>()
                );

                //  instructors 
                results.Instructors = new ObservableCollection<Instructor>(
                    instructors?.Where(i =>
                        i.Name?.ToLower().Contains(lowerQuery) == true ||
                        i.Email?.ToLower().Contains(lowerQuery) == true ||
                        i.Phone?.Contains(query) == true
                    ) ?? Enumerable.Empty<Instructor>()
                );

                // Search assessments
                results.Assessments = new ObservableCollection<Assessment>(
                    assessments?.Where(a =>
                        a.Name?.ToLower().Contains(lowerQuery) == true ||
                        a.Type?.ToString().ToLower().Contains(lowerQuery) == true ||
                        a.StartDate.ToString("MM/dd/yyyy").Contains(query) == true ||
                        a.EndDate.ToString("MM/dd/yyyy").Contains(query) == true
                    ) ?? Enumerable.Empty<Assessment>()
                );

                // Search notes
                results.Notes = new ObservableCollection<Note>(
                    notes?.Where(n =>
                        n.Body?.ToLower().Contains(lowerQuery) == true ||
                        n.Title?.ToLower().Contains(lowerQuery) == true
                    ) ?? Enumerable.Empty<Note>()
                );
            }
            catch (Exception ex)
            {
                // Log error or handle appropriately
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
            }

            return results;
        }
    }

    public class SearchResults
    {
        public ObservableCollection<Term> Terms { get; set; } = new();
        public ObservableCollection<Course> Courses { get; set; } = new();
        public ObservableCollection<Assessment> Assessments { get; set; } = new();
        public ObservableCollection<Note> Notes { get; set; } = new();
        public ObservableCollection<Instructor> Instructors { get; set; } = new(); // Add this


        public bool HasResults => Terms.Any() || Courses.Any() || Assessments.Any() || Notes.Any() || Instructors.Any();
        public int TotalResults => Terms.Count + Courses.Count + Assessments.Count + Notes.Count;
    }
}