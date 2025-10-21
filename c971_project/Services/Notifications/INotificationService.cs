namespace c971_project.Services.Notifications;
using c971_project.Core.Models;

public interface IScheduleNotificationService
{
    Task<bool> ScheduleCourseNotificationsAsync(Course course);
    Task<bool> ScheduleAssessmentNotificationsAsync(Assessment assessment);
}