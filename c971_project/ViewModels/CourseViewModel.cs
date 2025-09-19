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

    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class CourseViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int courseId;

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private Instructor instructor;

        [ObservableProperty]
        private ObservableCollection<Assessment> _assessments = new();

        [ObservableProperty]
        private ObservableCollection<Note> _notes = new();

        public CourseViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            WeakReferenceMessenger.Default.Register<AssessmentUpdatedMessage>(this, async (r, m) =>
            {
                await LoadAssessmentsAsync(); // reload from DB on assessment add
            });

            WeakReferenceMessenger.Default.Register<NoteUpdatedMessage>(this, async (r, m) =>
            {
                await LoadNotesAsync(); // reload from DB on note add
            });

            WeakReferenceMessenger.Default.Register<CourseUpdatedMessage>(this, async (r, m) =>
            {
                await LoadCourseAsync(CourseId); // reload from DB on Course edit
            });

        }

        partial void OnCourseIdChanged(int value)
        {
            _ = LoadDataAsync(value);

        }

        private async Task LoadDataAsync(int courseId)
        {
            await LoadCourseAsync(courseId);
            await LoadInstructorAsync();
            await LoadAssessmentsAsync();
            await LoadNotesAsync();

        }
        //load data
        private async Task LoadCourseAsync(int id)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Course = await _databaseService.GetCourseByIdAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Course: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task LoadInstructorAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Instructor = await _databaseService.GetInstructorByIdAsync(Course.InstructorId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Course: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadAssessmentsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (CourseId <= 0)
                    return;

                var assessmentList = await _databaseService.GetAssessmentsByCourseIdAsync(CourseId);

                Assessments.Clear();
                foreach (var assessment in assessmentList)
                    Assessments.Add(assessment);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading assessment data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadNotesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (CourseId <= 0)
                    return;

                var noteList = await _databaseService.GetNotesByCourseIdAsync(CourseId);

                Notes.Clear();
                foreach (var note in noteList)
                    Notes.Add(note);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading note data: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        //edit course content
        [RelayCommand]
        private async Task OnEditCourseAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(EditCoursePage),
                    new Dictionary<string, object> { { "CourseId", CourseId } });
            }
            finally { IsBusy = false; }
        }

        //assessment methods
        [RelayCommand]
        private async Task OnAddAssessmentAsync()
        {
            {
                if (IsBusy) return;
                try
                {
                    IsBusy = true;
                    await Shell.Current.GoToAsync(nameof(AddAssessmentPage),
                        new Dictionary<string, object> { { "CourseId", CourseId } });

                }
                finally { IsBusy = false; }
            }
        }

        [RelayCommand]
        private async Task OnDeleteAssessmentAsync(Assessment assessment)
        {
            if (IsBusy || assessment == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Assessment",
                $"Are you sure you want to delete '{assessment.Name}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _databaseService.DeleteAssessmentAsync(assessment);
                Assessments.Remove(assessment);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

        //notes methods
        [RelayCommand]
        private async Task OnDeleteNoteAsync(Note note)
        {
            if (IsBusy || note == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Note",
                $"Are you sure you want to delete '{note.Title}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _databaseService.DeleteNoteAsync(note);
                Notes.Remove(note);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task OnAddNoteAsync()
        {
            {
                if (IsBusy) return;
                try
                {
                    IsBusy = true;
                    await Shell.Current.GoToAsync(nameof(AddNotePage),
                        new Dictionary<string, object> { { "CourseId", CourseId } });

                }
                finally { IsBusy = false; }
            }
        }

        //[RelayCommand]
        //private async Task OnAssessmentPageAsync(Assessment assessment)
        //{
        //    if (IsBusy) return;
        //    try
        //    {
        //        IsBusy = true;
        //        await Shell.Current.GoToAsync(nameof(AssessmentPage),
        //            new Dictionary<string, object> { { "AssessmentId", assessment.AssessmentId } })
        //    }
        //    finally { IsBusy = false; }
        //}


    }
}
