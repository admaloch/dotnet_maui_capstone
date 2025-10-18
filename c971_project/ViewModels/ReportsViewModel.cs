// ViewModels/ReportsViewModel.cs
using c971_project.Services;
using c971_project.Services.Firebase;
using c971_project.Services.Reporting;
using c971_project.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace c971_project.ViewModels
{
    public partial class ReportsViewModel : BaseViewModel
    {
        private readonly IReportService _reportService;
        private readonly AuthService _authService;
        private readonly string _currentUserId;


        [ObservableProperty]
        private bool isGeneratingReport;

        [ObservableProperty]
        private string currentOperation;

        public ReportsViewModel(IReportService reportService, AuthService authService)
        {
            _reportService = reportService;
            _authService = authService;
            _currentUserId = _authService.CurrentUserId;
        }

        [RelayCommand]
        private async Task OnGenerateCourseReportAsync()
        {
            Debug.WriteLine("Starting course Report Generation");

            await GenerateReportAsync(
                operation: "Course Report",
                generateAction: () => _reportService.GenerateCourseReportAsync(_currentUserId)
            );
        }

        [RelayCommand]
        private async Task OnGenerateAssessmentReportAsync()
        {
            Debug.WriteLine("Starting Assessment Report Generation");
            await GenerateReportAsync(
                operation: "Assessment Report",
                generateAction: () => _reportService.GenerateAssessmentReportAsync(_currentUserId)
            );
        }

        [RelayCommand]
        private async Task OnGenerateTermReportAsync()
        {
            Debug.WriteLine("Starting Term Report Generation");
            await GenerateReportAsync(
                operation: "Term Report",
                generateAction: () => _reportService.GenerateTermReportAsync(_currentUserId)
            );
        }

        [RelayCommand]
        private async Task OnGenerateInstructorReportAsync()
        {
            Debug.WriteLine("Starting Instructor Report Generation");
            await GenerateReportAsync(
                operation: "Instructor Report",
                generateAction: () => _reportService.GenerateInstructorReportAsync(_currentUserId)
            );
        }

        [RelayCommand]
        private async Task OnGenerateComprehensiveReportAsync()
        {
            Debug.WriteLine("Starting Comprehensive Report Generation");
            await GenerateReportAsync(
                operation: "Comprehensive Report",
                generateAction: () => _reportService.GenerateComprehensiveReportAsync(_currentUserId)
            );
        }

        private async Task GenerateReportAsync(string operation, Func<Task<string>> generateAction)
        {
            if (IsGeneratingReport)
                return;

            IsGeneratingReport = true;
            CurrentOperation = $"Generating {operation}...";

            try
            {
                if (string.IsNullOrEmpty(_currentUserId))
                {
                    await Shell.Current.DisplayAlert("Error", "Please log in to generate reports", "OK");
                    return;
                }

                var filePath = await generateAction();

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    await ShareReportAsync(filePath, operation);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to generate report file", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to generate report: {ex.Message}", "OK");
                Debug.WriteLine($"Report generation error: {ex}");
            }
            finally
            {
                IsGeneratingReport = false;
                CurrentOperation = string.Empty;
            }
        }

        private async Task ShareReportAsync(string filePath, string title)
        {
            try
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = title,
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                // Sharing not supported, offer to save instead
                await Shell.Current.DisplayAlert("Share",
                    $"Report saved to: {filePath}\n\nSharing is not available on this device.", "OK");
                Debug.WriteLine($"Sharing error: {ex}");
            }
        }

      
    }
}