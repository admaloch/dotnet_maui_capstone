using c971_project.Services;
using c971_project.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;

namespace c971_project.Views;

public partial class HomePage : ContentPage, INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private Student _currentStudent = new Student();
    private ObservableCollection<Term> _terms = new ObservableCollection<Term>();

    public Student CurrentStudent
    {
        get => _currentStudent;
        set
        {
            _currentStudent = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<Term> Terms
    {
        get => _terms;
        set
        {
            _terms = value;
            OnPropertyChanged();
        }
    }

    public HomePage(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStudentDataAsync();
        Debug.WriteLine("HomePage Appearing");
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Debug.WriteLine("HomePage Disappearing");
    }

    private async Task LoadStudentDataAsync()
    {
        try
        {
            var students = await _databaseService.GetStudentsAsync();
            if (students != null && students.Count > 0)
            {
                CurrentStudent = students[0];
                await LoadTermsForCurrentStudentAsync();
            }
            else
            {
                CurrentStudent = new Student
                {
                    StudentId = "N/A",
                    Name = "No Student Data",
                    Email = "N/A",
                    Status = "Not Enrolled",
                    Major = "N/A"
                };
                Terms.Clear();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading student data: {ex.Message}");
        }
    }

    private async Task LoadTermsForCurrentStudentAsync()
    {
        if (string.IsNullOrEmpty(CurrentStudent?.StudentId))
            return;

        try
        {
            var terms = await _databaseService.GetTermsByStudentIdAsync(CurrentStudent.StudentId);

            Terms.Clear();
            if (terms != null)
            {
                foreach (var term in terms)
                {
                    Terms.Add(term);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading terms: {ex.Message}");
        }
    }

    // INotifyPropertyChanged implementation
    public new event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null!)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnAddTermClicked(object sender, EventArgs e)
    {
        Debug.WriteLine("Add Term clicked");
        // await Navigation.PushAsync(new AddEditTermPage(_databaseService));
    }
    private async void OnEditStudentClicked(object sender, EventArgs e)
    {
        if (CurrentStudent != null && !string.IsNullOrEmpty(CurrentStudent.StudentId))
        {
            await Shell.Current.GoToAsync($"{nameof(EditStudentPage)}?studentId={CurrentStudent.StudentId}");
        }
        else
        {
            await Shell.Current.GoToAsync(nameof(EditStudentPage));
        }
    }
}