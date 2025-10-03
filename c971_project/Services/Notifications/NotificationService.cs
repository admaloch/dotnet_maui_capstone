using c971_project.Models;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System.Diagnostics;

namespace c971_project.Services.Notifications;

public class NotificationService : IScheduleNotificationService
{
    public async Task<bool> ScheduleCourseNotificationsAsync(Course course)
    {
        try
        {
            Debug.WriteLine($"Current time: {DateTime.Now}");
            Debug.WriteLine($"Course start date {course.StartDate} -- Notification start date: {course.NotifyStartDate}");
            Debug.WriteLine($"Course end date {course.EndDate} -- Notification end date: {course.NotifyEndDate}");
            // Cancel any existing notifications for this course first
            await CancelCourseNotificationsAsync(course.CourseId);

            var results = new List<bool>();

            // Schedule start date notification if enabled
            if (course.NotifyStartDate && course.StartDate > DateTime.Now)  
            {
                var startNotification = new NotificationRequest
                {
                    NotificationId = GenerateCourseNotificationId(course.CourseId, isStartDate: true),
                    Title = "Course Starting Soon!",
                    Description = $"{course.Name} starts on {course.StartDate:MMM dd, yyyy 'at' h:mm tt}",  // Updated
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = course.StartDate  // 🔄 CHANGED: Use the combined date+time
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "course_alerts",
                        AutoCancel = true
                    }
                };

                var startResult = await LocalNotificationCenter.Current.Show(startNotification);
                results.Add(startResult);
                Debug.WriteLine($"Start date notification scheduled for: {course.StartDate:MMM dd, yyyy h:mm tt} - Result: {startResult}");  // Updated
            }

            // Schedule end date notification if enabled
            if (course.NotifyEndDate && course.EndDate > DateTime.Now)  // Changed to EndDateTime
            {
                var endNotification = new NotificationRequest
                {
                    NotificationId = GenerateCourseNotificationId(course.CourseId, isStartDate: false),
                    Title = "Course Ending Soon!",
                    Description = $"{course.Name} ends on {course.EndDate:MMM dd, yyyy 'at' h:mm tt}",  // Updated
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = course.EndDate  // 🔄 CHANGED: Use the combined date+time
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "course_alerts",
                        AutoCancel = true
                    }
                };

                var endResult = await LocalNotificationCenter.Current.Show(endNotification);
                results.Add(endResult);
                Debug.WriteLine($"End date notification scheduled for: {course.EndDate:MMM dd, yyyy h:mm tt} - Result: {endResult}");  // Updated
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
            Debug.WriteLine($"Current time: {DateTime.Now}");
            Debug.WriteLine($"Assessment start date time {assessment.StartDateTime} -- Start  notification toggle: {assessment.NotifyStartDate}");
            Debug.WriteLine($"Assessment end date time {assessment.EndDateTime} -- End notification toggle: {assessment.NotifyEndDate}");


            await CancelAssessmentNotificationsAsync(assessment.AssessmentId);

            var results = new List<bool>();

            // ✅ FIX: Compare against StartDateTime instead of StartDate
            if (assessment.NotifyStartDate && assessment.StartDateTime > DateTime.Now)
            {

                var startNotification = new NotificationRequest
                {
                    NotificationId = GenerateAssessmentNotificationId(assessment.AssessmentId, isStartDate: true),
                    Title = "Assessment Available!",
                    Description = $"{assessment.Name} starts on {assessment.StartDate:MMM dd, yyyy}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = assessment.StartDateTime
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "assessment_alerts",
                        AutoCancel = true
                    }
                };

                var startResult = await LocalNotificationCenter.Current.Show(startNotification);
                results.Add(startResult);
                Debug.WriteLine($"Start notification SUCCESS - scheduled for {assessment.StartDateTime:MMM dd, yyyy h:mm tt} - Result: {startResult}");
            }
            else
            {
                Debug.WriteLine("❌ START notification Failed - conditions not met");

            }

            // ✅ FIX: Compare against EndDateTime instead of EndDate
            if (assessment.NotifyEndDate && assessment.EndDateTime > DateTime.Now)
            {

                var endNotification = new NotificationRequest
                {
                    NotificationId = GenerateAssessmentNotificationId(assessment.AssessmentId, isStartDate: false),
                    Title = "Assessment Due Soon!",
                    Description = $"{assessment.Name} is due on {assessment.EndDate:MMM dd, yyyy}",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = assessment.EndDateTime
                    },
                    Android = new AndroidOptions
                    {
                        ChannelId = "assessment_alerts",
                        AutoCancel = true
                    }
                };

                var endResult = await LocalNotificationCenter.Current.Show(endNotification);
                results.Add(endResult);
                Debug.WriteLine($"End notification SUCCESS - scheduled for {assessment.EndDateTime:MMM dd, yyyy h:mm tt} - Result: {endResult}");
            }
            else
            {
                Debug.WriteLine("❌ END notification FAILED - conditions not met");
            }

            if (results.Count == 0)
            {
                Debug.WriteLine("📭 No notifications were scheduled");
                return true;
            }
            else
            {
                var success = results.Any(r => r);
                Debug.WriteLine($"📋 Notification summary: {results.Count} attempted, {results.Count(r => r)} successful");
                return success;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Assessment notification try/catch FAILED - Message: {ex.Message}");
            return false;
        }
    }
    private async Task CancelCourseNotificationsAsync(int courseId)
    {
        try
        {
            LocalNotificationCenter.Current.Cancel(GenerateCourseNotificationId(courseId, isStartDate: true));
            LocalNotificationCenter.Current.Cancel(GenerateCourseNotificationId(courseId, isStartDate: false));
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
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error cancelling assessment notifications: {ex.Message}");
        }
    }

    private int GenerateCourseNotificationId(int courseId, bool isStartDate)
    {
        return courseId * 100 + (isStartDate ? 1 : 2);
    }

    private int GenerateAssessmentNotificationId(int assessmentId, bool isStartDate)
    {
        return assessmentId * 100 + (isStartDate ? 3 : 4);
    }
}