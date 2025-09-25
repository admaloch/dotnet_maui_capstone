using c971_project.Models;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System.Diagnostics;

namespace c971_project.Services;

public class NotificationService : IScheduleNotificationService
{
    public async Task<bool> ScheduleCourseNotificationsAsync(Course course)
    {
        try
        {
            Debug.WriteLine($"=== Scheduling notifications for: {course.Name} ===");

            // Cancel any existing notifications for this course first
            await CancelCourseNotificationsAsync(course.CourseId);

            var results = new List<bool>();

            // Schedule start date notification if enabled
            if (course.NotifyStartDate && course.StartDateTime > DateTime.Now)  // Changed to StartDateTime
            {
                var startNotification = new NotificationRequest
                {
                    NotificationId = GenerateCourseNotificationId(course.CourseId, isStartDate: true),
                    Title = "Course Starting Soon!",
                    Description = $"{course.Name} starts on {course.StartDateTime:MMM dd, yyyy 'at' h:mm tt}",  // Updated
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = course.StartDateTime  // 🔄 CHANGED: Use the combined date+time
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "course_alerts",
                        AutoCancel = true
                    }
                };

                var startResult = await LocalNotificationCenter.Current.Show(startNotification);
                results.Add(startResult);
                Debug.WriteLine($"Start date notification scheduled for: {course.StartDateTime:MMM dd, yyyy h:mm tt} - Result: {startResult}");  // Updated
            }

            // Schedule end date notification if enabled
            if (course.NotifyEndDate && course.EndDateTime > DateTime.Now)  // Changed to EndDateTime
            {
                var endNotification = new NotificationRequest
                {
                    NotificationId = GenerateCourseNotificationId(course.CourseId, isStartDate: false),
                    Title = "Course Ending Soon!",
                    Description = $"{course.Name} ends on {course.EndDateTime:MMM dd, yyyy 'at' h:mm tt}",  // Updated
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = course.EndDateTime  // 🔄 CHANGED: Use the combined date+time
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "course_alerts",
                        AutoCancel = true
                    }
                };

                var endResult = await LocalNotificationCenter.Current.Show(endNotification);
                results.Add(endResult);
                Debug.WriteLine($"End date notification scheduled for: {course.EndDateTime:MMM dd, yyyy h:mm tt} - Result: {endResult}");  // Updated
            }

            // If no notifications were scheduled (dates in past or toggles off)
            if (results.Count == 0)
            {
                Debug.WriteLine("No notifications scheduled - dates may be in past or notifications disabled");
                return true; // Still return true since it's not an error
            }

            // Return true if at least one notification was scheduled successfully
            var success = results.Any(r => r);
            Debug.WriteLine($"Overall notification scheduling success: {success}");

            return success;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"💥 Error scheduling notifications: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ScheduleAssessmentNotificationsAsync(Assessment assessment)
    {
        try
        {
            Debug.WriteLine($"=== Scheduling assessment notifications for: {assessment.Name} ===");

            await CancelAssessmentNotificationsAsync(assessment.AssessmentId);

            var results = new List<bool>();

            if (assessment.NotifyStartDate && assessment.StartDate > DateTime.Now)
            {
                var startNotification = new NotificationRequest
                {
                    NotificationId = GenerateAssessmentNotificationId(assessment.AssessmentId, isStartDate: true),
                    Title = "Assessment Available!",
                    Description = $"{assessment.Name} starts on {assessment.StartDate:MMM dd, yyyy}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = assessment.StartDateTime // uses combined date+time
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "assessment_alerts",
                        AutoCancel = true
                    }
                };

                var startResult = await LocalNotificationCenter.Current.Show(startNotification);
                results.Add(startResult);
                Debug.WriteLine($"Assessment start notification scheduled - Result: {startResult}");
            }

            if (assessment.NotifyEndDate && assessment.EndDate > DateTime.Now)
            {
                var endNotification = new NotificationRequest
                {
                    NotificationId = GenerateAssessmentNotificationId(assessment.AssessmentId, isStartDate: false),
                    Title = "Assessment Due Soon!",
                    Description = $"{assessment.Name} is due on {assessment.EndDate:MMM dd, yyyy}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = assessment.StartDateTime // NEW - uses the combined date+time
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "assessment_alerts",
                        AutoCancel = true
                    }
                };

                var endResult = await LocalNotificationCenter.Current.Show(endNotification);
                results.Add(endResult);
                Debug.WriteLine($"Assessment end notification scheduled - Result: {endResult}");
            }

            return results.Count == 0 || results.Any(r => r);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"💥 Error scheduling assessment notifications: {ex.Message}");
            return false;
        }
    }

    private async Task CancelCourseNotificationsAsync(int courseId)
    {
        try
        {
            LocalNotificationCenter.Current.Cancel(GenerateCourseNotificationId(courseId, isStartDate: true));
            LocalNotificationCenter.Current.Cancel(GenerateCourseNotificationId(courseId, isStartDate: false));
            Debug.WriteLine($"Cancelled existing notifications for course ID: {courseId}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cancelling course notifications: {ex.Message}");
        }
    }

    private async Task CancelAssessmentNotificationsAsync(int assessmentId)
    {
        try
        {
            LocalNotificationCenter.Current.Cancel(GenerateAssessmentNotificationId(assessmentId, isStartDate: true));
            LocalNotificationCenter.Current.Cancel(GenerateAssessmentNotificationId(assessmentId, isStartDate: false));
            Debug.WriteLine($"Cancelled existing notifications for assessment ID: {assessmentId}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cancelling assessment notifications: {ex.Message}");
        }
    }

    private int GenerateCourseNotificationId(int courseId, bool isStartDate)
    {
        return (courseId * 100) + (isStartDate ? 1 : 2);
    }

    private int GenerateAssessmentNotificationId(int assessmentId, bool isStartDate)
    {
        return (assessmentId * 100) + (isStartDate ? 3 : 4);
    }
}