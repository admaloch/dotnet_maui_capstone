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
    [QueryProperty(nameof(CourseId), "CourseId")]

    public partial class EditCourseViewModel : BaseViewModel
    {

        [ObservableProperty]
        private int courseId;

        [ObservableProperty]
        private Course _course;

        private readonly DatabaseService _databaseService;

        public EditCourseViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        partial void OnCourseIdChanged(int value)
        {
            LoadCourseAsync(value); // call async fire-and-forget
        }

        private async Task LoadCourseAsync(int courseId)
        {
            Course = await _databaseService.GetCourseByIdAsync(courseId);
        }





    }
}
