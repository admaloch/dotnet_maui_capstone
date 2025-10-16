using c971_project.Messages;
using c971_project.Models;
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


namespace c971_project.ViewModels
{

    [QueryProperty(nameof(NoteId), "NoteId")]
    public partial class NoteViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string noteId;

        [ObservableProperty]
        private Note note;

        [ObservableProperty]
        private Course course;



        public NoteViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;

            WeakReferenceMessenger.Default.Register<NoteUpdatedMessage>(this, async (r, m) =>
            {
                await LoadNoteAsync(NoteId); // reload from DB on Note edit
            });
        }

        partial void OnNoteIdChanged(string value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(string noteId)
        {
            await LoadNoteAsync(noteId);
            await LoadCourseAsync();
        }


        //load note data
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

        //load course data for note
        private async Task LoadCourseAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Course = await _firestoreDataService.GetCourseAsync(Note.CourseId);
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

        //edit note content
        [RelayCommand]
        private async Task OnEditNoteAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(EditNotePage),
                    new Dictionary<string, object> { { "NoteId", NoteId } });
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task OnDeleteNoteAsync()
        {
            if (IsBusy || Note == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Note",
                $"Are you sure you want to delete '{Note.Title}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                await _firestoreDataService.DeleteNoteAsync(Note.Id);
                WeakReferenceMessenger.Default.Send(new NoteUpdatedMessage());
                await Shell.Current.GoToAsync("..");

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }
        }

        //share note -- built in share feature
        [RelayCommand]
        private async Task ShareNote()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // Check if there are notes to share
                if (string.IsNullOrWhiteSpace(Note.Body))
                {
                    await Shell.Current.DisplayAlert("No Notes", "Please add some notes before sharing.", "OK");
                    return;
                }

                // Prepare the text to share
                var shareText = GenerateShareText();

                // Use MAUI's Share API
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = $"Share {Note.Title} note for {Course.Name} -- {Course.CourseNum}"
                });

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Unable to share notes: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private string GenerateShareText()
        {
            return $"""
            Course Notes: {Note.Title}
            Course: {Course.Name}
            Course Number: {Course.CourseNum}
            Date Created: {Note.DateAdded:MMM dd, yyyy}
            Last Updated: {Note.LastUpdated:MMM dd, yyyy}
           
            NOTES:
            {Note.Body}
            
            ---
            Shared from Course Tracker App
            """;
        }
    }
}
