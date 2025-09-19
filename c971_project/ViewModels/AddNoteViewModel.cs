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

            // Initialize the new note
            NewNote = new Note
            {
                Title = string.Empty,
                Body = string.Empty,
                CourseId = courseId,
                DateAdded = DateTime.Today,
                LastUpdated = DateTime.Today
            };
        }

        [RelayCommand]
        private async Task SaveNoteAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var isTaskValid = await ValidateNoteAsync();

                if (!isTaskValid)
                    return;

                // Save to database
                await _databaseService.SaveNoteAsync(NewNote);

                // Optional: notify other viewmodels
                WeakReferenceMessenger.Default.Send(new NoteUpdatedMessage());

                await Shell.Current.DisplayAlert("Success", "Note saved successfully.", "OK");
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

        private async Task<bool> ValidateNoteAsync()
        {
            NewNote.Validate();

            if (NewNote.HasErrors)
            {
                var errorMessage = ValidationHelper.GetErrors(
                    NewNote,
                    nameof(Note.Title),
                    nameof(Note.Body)
                );

                await Shell.Current.DisplayAlert("Validation Errors", errorMessage, "OK");
                return false;
            }
            return true;
        }

    }
}
