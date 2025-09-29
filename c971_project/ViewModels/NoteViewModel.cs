using c971_project.Messages;
using c971_project.Models;
using c971_project.Services.Data;
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
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int noteId;

        [ObservableProperty]
        private Note note;

        [ObservableProperty]
        private Course course;



        public NoteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            WeakReferenceMessenger.Default.Register<NoteUpdatedMessage>(this, async (r, m) =>
            {
                await LoadNoteAsync(NoteId); // reload from DB on Note edit
            });
        }

        partial void OnNoteIdChanged(int value)
        {
            _ = LoadDataAsync(value);
        }

        private async Task LoadDataAsync(int noteId)
        {
            await LoadNoteAsync(noteId);
            await LoadCourseAsync();
        }


        //load note data
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

        //load course data for note
        private async Task LoadCourseAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Course = await _databaseService.GetCourseByIdAsync(Note.CourseId);
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
