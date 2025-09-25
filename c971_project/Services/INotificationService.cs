namespace c971_project.Services;
using c971_project.Models;

public interface IScheduleNotificationService
{
    Task<bool> ScheduleCourseNotificationsAsync(Course course);
    Task<bool> ScheduleAssessmentNotificationsAsync(Assessment assessment);
}