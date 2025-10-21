using c971_project.Models;
using c971_project.Helpers;
using c971_project.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using c971_project.Services.ValidationServices;
using c971_project.Services.Firebase;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class AddNoteViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly IAuthService _authService;
        private string _currentUserId;

        [ObservableProperty]
        private string courseId;

        [ObservableProperty]
        private Note _newNote;

        public AddNoteViewModel(IFirestoreDataService firestoreDataService, IAuthService authService)
        {
            _firestoreDataService = firestoreDataService;
            _authService = authService;
            _currentUserId = _authService.CurrentUserId;
        }

        // This runs when CourseId is assigned by Shell after navigation
        partial void OnCourseIdChanged(string value)
        {
            InitializeDefaultNote(value);
        }

        private void InitializeDefaultNote(string courseId)
        {
            if (NewNote == null)
            {
                NewNote = new Note
                {
                    Title = string.Empty,
                    UserId = _currentUserId,
                    Body = string.Empty,
                    CourseId = courseId,
                    DateAdded = DateTime.Today,
                    LastUpdated = DateTime.Today
                };
            }
            else
            {
                NewNote.CourseId = courseId;
            }
        }

        [RelayCommand]
        private async Task SaveNoteAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // validate -- if errors -- print and return
                var errors = NoteValidator.ValidateNote(NewNote).ToString().Trim();
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                // Save to database
                await _firestoreDataService.SaveNoteAsync(NewNote);

                // notify change
                WeakReferenceMessenger.Default.Send(new NoteUpdatedMessage());

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving note: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to save note. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
