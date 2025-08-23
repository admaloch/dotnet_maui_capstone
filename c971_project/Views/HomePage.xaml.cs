using c971_project.Services;
using c971_project.Models;
using c971_project.Extensions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace c971_project.Views
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Student _currentStudent;
        private ObservableCollection<Term> _terms;

        public Student CurrentStudent
        {
            get => _currentStudent;
            set
            {
                _currentStudent = value;
                OnPropertyChanged(nameof(CurrentStudent));
            }
        }

        public ObservableCollection<Term> Terms
        {
            get => _terms;
            set
            {
                _terms = value;
                OnPropertyChanged(nameof(Terms));
            }
        }

        public HomePage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;

            // Initialize the terms collection
            Terms = new ObservableCollection<Term>();

            // Set binding context
            BindingContext = this;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadStudentDataAsync(); // Refresh data when page appears
        }

        private async Task LoadStudentDataAsync()
        {
            var students = await _databaseService.GetStudentsAsync();
            if (students.Any())
            {
                CurrentStudent = students.First();
                await LoadTermsForCurrentStudentAsync();
            }
            else
            {
                // Fallback if no student data
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

        private async Task LoadTermsForCurrentStudentAsync()
        {
            if (CurrentStudent == null || string.IsNullOrEmpty(CurrentStudent.StudentId))
                return;

            var terms = await _databaseService.GetTermsByStudentIdAsync(CurrentStudent.StudentId);

            // Update the observable collection on the UI thread
            Terms.Clear();
            foreach (var term in terms)
            {
                Terms.Add(term);
            }
        }

        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler PropertyChanged; //creates "station" that UI controls tune into

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}