using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
using c971_project.ViewModels;
using c971_project.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace c971_project.ViewModels
{

    [QueryProperty(nameof(TermId), "TermId")]
    public partial class TermViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private Term term;

        [ObservableProperty]
        private ObservableCollection<Course> _courses = new();

        public TermViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            WeakReferenceMessenger.Default.Register<CourseUpdatedMessage>(this, async (r, m) =>
            {
                await LoadCoursesAsync(); // reload from DB on course add
            });
            WeakReferenceMessenger.Default.Register<TermUpdatedMessage>(this, async (r, m) =>
            {
                await LoadTermAsync(TermId); // reload from DB on term add
            });

        }

        partial void OnTermIdChanged(int value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(int termId)
        {
            await LoadTermAsync(termId);
            await LoadCoursesAsync();
        }

        private async Task LoadTermAsync(int id)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Term = await _databaseService.GetTermByIdAsync(id);
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

                if (TermId <= 0)
                    return;

                var courseList = await _databaseService.GetCoursesByTermIdAsync(TermId);

                Courses.Clear();
                foreach (var course in courseList)
                    Courses.Add(course);

                Debug.WriteLine($"Total courses in DB: {Courses.Count}");
                foreach (var c in Courses)
                    Debug.WriteLine($"Course: {c.CourseId}, {c.Name}, TermId={c.TermId}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading course data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
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
                if (IsBusy) return;
                try
                {
                    IsBusy = true;
                    await Shell.Current.GoToAsync(nameof(AddCoursePage),
                        new Dictionary<string, object> { { "TermId", Term.TermId } });
                }
                finally { IsBusy = false; }
            }
        }
        [RelayCommand]
        private async Task OnDeleteCourseAsync(Course course)
        {
            if (IsBusy || course == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Course",
                $"Are you sure you want to delete '{course.CourseNum} - {course.Name}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                //delete course and corresponding notes and assessments
                await _databaseService.DeleteAssessmentsByCourseIdAsync(course.CourseId);
                await _databaseService.DeleteNotesByCourseIdAsync(course.CourseId);
                await _databaseService.DeleteCourseAsync(course);
                Courses.Remove(course);
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
                    new Dictionary<string, object> { { "CourseId", course.CourseId } });
            }
            finally { IsBusy = false; }
        }
    }
 }
