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

    [QueryProperty(nameof(AssessmentId), "AssessmentId")]
    public partial class AssessmentViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string assessmentId;

        [ObservableProperty]
        private Assessment assessment;

        [ObservableProperty]
        private Course course;



        public AssessmentViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;

            WeakReferenceMessenger.Default.Register<AssessmentUpdatedMessage>(this, async (r, m) =>
            {
                await LoadAssessmentAsync(AssessmentId); // reload from DB on Assessment edit
            });
        }

        partial void OnAssessmentIdChanged(string value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(string assessmentId)
        {
            await LoadAssessmentAsync(assessmentId);
            await LoadCourseAsync();
        }

        //load  data
        private async Task LoadAssessmentAsync(string id)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Assessment = await _firestoreDataService.GetAssessmentAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Assessment: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task LoadCourseAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Course = await _firestoreDataService.GetCourseAsync(Assessment.CourseId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Assessment: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        //ASSESSMENT METHODS
        [RelayCommand]
        private async Task OnEditAssessmentAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(EditAssessmentPage),
                    new Dictionary<string, object> { { "AssessmentId", AssessmentId } });
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task OnDeleteAssessmentAsync()
        {
            if (IsBusy || Assessment == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Assessment",
                $"Are you sure you want to delete '{Assessment.Name}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _firestoreDataService.DeleteAssessmentAsync(Assessment.Id);
                WeakReferenceMessenger.Default.Send(new AssessmentUpdatedMessage());
                await Shell.Current.GoToAsync("..");

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

       
    }
}
