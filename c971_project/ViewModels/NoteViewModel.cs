using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
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
            _ = LoadNoteAsync(value);
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
    }
}
