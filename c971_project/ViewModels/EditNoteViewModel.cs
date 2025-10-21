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
    [QueryProperty(nameof(NoteId), "NoteId")]
    public partial class EditNoteViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly IAuthService _authService;
        private string _currentUserId;


        [ObservableProperty]
        private string noteId;

        [ObservableProperty]
        private Note _note;

        public EditNoteViewModel(IFirestoreDataService firestoreDataService, IAuthService authService)
        {
            _firestoreDataService = firestoreDataService;
            _authService = authService;
            _currentUserId = _authService.CurrentUserId;
        }

        partial void OnNoteIdChanged(string value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(string noteId)
        {
            await LoadNoteAsync(noteId);
        }

        //load data
        private async Task LoadNoteAsync(string id)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Note = await _firestoreDataService.GetNoteAsync(id);
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
            Note.UserId = _currentUserId;

            try
            {
                IsBusy = true;

                Note.LastUpdated = DateTime.Now;

                // validate -- if errors -- print and return
                var errors = NoteValidator.ValidateNote(Note).ToString().Trim();
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                // Save to database
                await _firestoreDataService.SaveNoteAsync(Note);

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
