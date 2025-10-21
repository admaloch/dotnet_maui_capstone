using c971_project.Messages;
using c971_project.Core.Models;
using c971_project.Services.Firebase;
using c971_project.ViewModels;
using c971_project.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;
using c971_project.Core.Services;

namespace c971_project.ViewModels
{

    [QueryProperty(nameof(TermId), "TermId")]
    public partial class TermViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string termId;

        [ObservableProperty]
        private Term term;

        [ObservableProperty]
        private ObservableCollection<Course> _courses = new();
        public bool CanAddMoreCourses => Courses.Count < 6;

        public TermViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;

            // Notify when Courses collection changes
            Courses.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanAddMoreCourses));
            };

            WeakReferenceMessenger.Default.Register<CourseUpdatedMessage>(this, async (r, m) =>
            {
                await LoadCoursesAsync(); // reload from DB on course add
            });
            WeakReferenceMessenger.Default.Register<TermUpdatedMessage>(this, async (r, m) =>
            {
                await LoadTermAsync(TermId); // reload from DB on term add
            });

        }

        partial void OnTermIdChanged(string value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(string termId)
        {
            await LoadTermAsync(termId);
            await LoadCoursesAsync();
        }

        private async Task LoadTermAsync(string id)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Term = await _firestoreDataService.GetTermAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Term: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        public async Task LoadCoursesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrEmpty(TermId)) return;

                var courseList = await _firestoreDataService.GetCoursesByTermIdAsync(TermId);

                Courses.Clear();
                foreach (var course in courseList)
                    Courses.Add(course);

                Debug.WriteLine($"Total courses in DB: {Courses.Count}");
                foreach (var c in Courses)
                    Debug.WriteLine($"Course: {c.Id}, {c.Name}, TermId={c.TermId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading course data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
            // update on add courses
            OnPropertyChanged(nameof(CanAddMoreCourses));
        }
        [RelayCommand]
        private async Task OnEditTermAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(EditTermPage),
                    new Dictionary<string, object> { { "TermId", TermId } });

            }
            finally { IsBusy = false; }
        }
        [RelayCommand]
        private async Task OnAddCourseAsync()
        {
            {
                if (!CanAddMoreCourses)
                {
                    await Shell.Current.DisplayAlert("Notification", "You have reached the maximum 6 courses", "OK");
                    return;
                }

                if (IsBusy) return;
                try
                {
                    IsBusy = true;
                    await Shell.Current.GoToAsync(nameof(AddCoursePage),
                        new Dictionary<string, object> { { "TermId", Term.Id } });
                }
                finally { IsBusy = false; }
            }
        }

        [RelayCommand]
        private async Task OnDeleteTermAsync()
        {
            if (IsBusy || Term.Name == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Term",
                $"Are you sure you want to delete '{Term.Name}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                //also delete courses - notes - assessments
                var courses = await _firestoreDataService.GetCoursesByTermIdAsync(TermId);
                foreach (var course in courses)
                {
                    await _firestoreDataService.DeleteAssessmentsByCourseIdAsync(course.Id);
                    await _firestoreDataService.DeleteNotesByCourseIdAsync(course.Id);
                    await _firestoreDataService.DeleteCourseAsync(course.Id);
                }
                await _firestoreDataService.DeleteTermAsync(TermId);

                WeakReferenceMessenger.Default.Send(new TermUpdatedMessage());

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }

        }
        [RelayCommand]
        private async Task OnCoursePageAsync(Course course)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(CoursePage),
                    new Dictionary<string, object> { { "CourseId", course.Id } });
            }
            finally { IsBusy = false; }
        }
    }
 }
