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
    [QueryProperty(nameof(NoteId), "NoteId")]
    public partial class EditNoteViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int noteId;

        [ObservableProperty]
        private Note _note;

        public EditNoteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        partial void OnNoteIdChanged(int value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(int noteId)
        {
            await LoadNoteAsync(noteId);
        }

        //load data
        private async Task LoadNoteAsync(int id)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Note = await _databaseService.GetNoteByIdAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Note: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveNoteAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                Note.LastUpdated = DateTime.Now;

                var isTaskValid = await ValidateNoteAsync();

                if (!isTaskValid)
                    return;

                // Save to database
                await _databaseService.SaveNoteAsync(Note);

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
            Note.Validate();

            if (Note.HasErrors)
            {
                var errorMessage = ValidationHelper.GetErrors(
                    Note,
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
