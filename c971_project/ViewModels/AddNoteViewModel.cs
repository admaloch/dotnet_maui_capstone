using c971_project.Models;
using c971_project.Helpers;
using c971_project.Messages;

using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class AddNoteViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int courseId;

        [ObservableProperty]
        private Note _newNote;

        public AddNoteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // This runs when CourseId is assigned by Shell after navigation
        partial void OnCourseIdChanged(int value)
        {
            InitializeDefaultNote(value);
        }

        private void InitializeDefaultNote(int courseId)
        {
            if (NewNote == null)
            {
                NewNote = new Note
                {
                    Title = string.Empty,
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
                await _databaseService.SaveNoteAsync(NewNote);

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
