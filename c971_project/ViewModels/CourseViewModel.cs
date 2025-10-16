using c971_project.Messages;
using System.Collections.Specialized;
using c971_project.Models;
using c971_project.ViewModels;
using c971_project.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;


using System.Threading.Tasks;
using c971_project.Services.Firebase;


namespace c971_project.ViewModels
{

    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class CourseViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string courseId;

        [ObservableProperty]
        private Course course;

        [ObservableProperty]
        private Instructor instructor;

        [ObservableProperty]
        private ObservableCollection<Assessment> _assessments = new();

        [ObservableProperty]
        private ObservableCollection<Note> _notes = new();

        public bool CanAddMoreAssessments
        {
            get
            {
                var objectiveCount = Assessments.Count(a => a.Type == "Objective");
                var performanceCount = Assessments.Count(a => a.Type == "Performance");

                return objectiveCount < 1 || performanceCount < 1;
            }
        }

        public CourseViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;

            // Notify when Assessment collection changes
            Assessments.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanAddMoreAssessments));
            };

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
                await LoadInstructorAsync();

            });
        }

        partial void OnCourseIdChanged(string value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(string courseId)
        {
            await LoadCourseAsync(courseId);
            await LoadInstructorAsync();
            await LoadAssessmentsAsync();
            await LoadNotesAsync();

        }
        //load data
        private async Task LoadCourseAsync(string id)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Course = await _firestoreDataService.GetCourseAsync(id);
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
                Instructor = await _firestoreDataService.GetInstructorAsync(Course.InstructorId);
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

                if (string.IsNullOrEmpty(CourseId))
                    return;

                var assessmentList = await _firestoreDataService.GetAssessmentsByCourseIdAsync(CourseId);

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
            // update on add courses
            OnPropertyChanged(nameof(CanAddMoreAssessments));
        }

        public async Task LoadNotesAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrEmpty(CourseId))
                    return;

                var noteList = await _firestoreDataService.GetNotesByCourseIdAsync(CourseId);

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
                if (!CanAddMoreAssessments)
                {
                    await Shell.Current.DisplayAlert("Notification", "You have reached the maximum 2 assessments", "OK");
                    return;
                }

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
        private async Task OnDeleteCourseAsync()
        {
            if (IsBusy || Course == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Course",
                $"Are you sure you want to delete '{Course.CourseNum} - {Course.Name}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                //delete Course and corresponding notes and assessments
                await _firestoreDataService.DeleteAssessmentsByCourseIdAsync(Course.Id);
                await _firestoreDataService.DeleteNotesByCourseIdAsync(Course.Id);
                await _firestoreDataService.DeleteCourseAsync(Course.Id);
                WeakReferenceMessenger.Default.Send(new CourseUpdatedMessage());
                await Shell.Current.GoToAsync("..");

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }

        }

        [RelayCommand]
        private async Task OnEditAssessmentAsync(Assessment assessment)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(EditAssessmentPage),
                    new Dictionary<string, object> { { "AssessmentId", assessment.Id } });
            }
            finally { IsBusy = false; }
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
                await _firestoreDataService.DeleteAssessmentAsync(assessment.Id);
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

        [RelayCommand]
        private async Task OnNotePageAsync(Note note)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(NotePage),
                    new Dictionary<string, object> { { "NoteId", note.Id } });
            }
            finally { IsBusy = false; }
        }


    }
}
