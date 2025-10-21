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

    [QueryProperty(nameof(InstructorId), "InstructorId")]
    public partial class InstructorViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string instructorId;

        [ObservableProperty]
        private Instructor instructor;

        [ObservableProperty]
        private Course course;



        public InstructorViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;

        }

        partial void OnInstructorIdChanged(string value)
        {
            _ = LoadInstructorAsync(value);
        }

        //load instructor data
        private async Task LoadInstructorAsync(string id)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Instructor = await _firestoreDataService.GetInstructorAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Instructor: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

     

        

     

      

      
    }
}
