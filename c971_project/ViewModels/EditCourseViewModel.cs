using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Diagnostics;

namespace c971_project.ViewModels
{
    //[QueryProperty(nameof(CourseId), "CourseId")]

    public partial class EditCourseViewModel : BaseViewModel
    {


        [ObservableProperty]
        private Course course;

        private readonly DatabaseService _databaseService;

        public EditCourseViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

           
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync(".."); // navigate back
        }




    }
}
