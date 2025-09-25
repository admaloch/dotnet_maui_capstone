namespace c971_project.Services;
using c971_project.Models;

public interface ICourseNotificationService
{
    Task<bool> ScheduleCourseNotificationsAsync(Course course);
    Task<bool> ScheduleAssessmentNotificationsAsync(Assessment assessment);
}