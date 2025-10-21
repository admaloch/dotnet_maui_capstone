// ViewModels/SearchViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using c971_project.Models;
using c971_project.Views;

using c971_project.Services.Firebase;
using c971_project.Services.Search;


namespace c971_project.ViewModels
{
    public partial class SearchViewModel : BaseViewModel
    {
        private readonly SearchService _searchService;
        private readonly IAuthService _authService;
        private string _currentUserId;


        [ObservableProperty]
        private string searchQuery;

        [ObservableProperty]
        private SearchResults searchResults;

        [ObservableProperty]
        private bool hasSearched;

        public SearchViewModel(SearchService searchService, IAuthService authService)
        {
            _searchService = searchService;
            _authService = authService;

            SearchResults = new SearchResults();
        }

        [RelayCommand]
        private async Task SearchAsync()
        {
            _currentUserId = _authService.CurrentUserId;

            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                await Shell.Current.DisplayAlert("Search", "Please enter a search term", "OK");
                return;
            }

            IsBusy = true;
            HasSearched = true;

            try
            {
                
                if (string.IsNullOrEmpty(_currentUserId))
                {
                    await Shell.Current.DisplayAlert("Error", "Please log in to search", "OK");
                    return;
                }

                SearchResults = await _searchService.SearchAsync(SearchQuery, _currentUserId);

                if (!SearchResults.HasResults)
                {
                    await Shell.Current.DisplayAlert("Search", "No results found", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
                SearchResults = new SearchResults();
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToResultAsync(object item)
        {
            try
            {
                switch (item)
                {
                    case Term term:
                        await Shell.Current.GoToAsync($"{nameof(TermPage)}?TermId={term.Id}");
                        break;
                    case Course course:
                        await Shell.Current.GoToAsync($"{nameof(CoursePage)}?CourseId={course.Id}");
                        break;
                    case Assessment assessment:
                        await Shell.Current.GoToAsync($"{nameof(AssessmentPage)}?AssessmentId={assessment.Id}");
                        break;
                    case Note note:
                        await Shell.Current.GoToAsync($"{nameof(NotePage)}?NoteId={note.Id}");
                        break;
                    case Instructor instructor:
                        await Shell.Current.GoToAsync($"{nameof(InstructorPage)}?InstructorId={instructor.Id}");
                        break;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Navigation Error", $"Could not navigate to item: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void ClearSearch()
        {
            SearchQuery = string.Empty;
            SearchResults = new SearchResults();
            HasSearched = false;
        }
    }
}