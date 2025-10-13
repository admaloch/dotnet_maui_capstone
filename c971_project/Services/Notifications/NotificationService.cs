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
            Debug.WriteLine($"=== Scheduling course notifications for: {course.Name} ===");
            Debug.WriteLine($"Current time: {DateTime.Now}");
            Debug.WriteLine($"Course ID: {course.Id}");
            Debug.WriteLine($"StartDate: {course.StartDate} | StartDateTime: {course.StartDateTime}");
            Debug.WriteLine($"EndDate: {course.EndDate} | EndDateTime: {course.EndDateTime}");
            Debug.WriteLine($"NotifyStart: {course.NotifyStartDate} | NotifyEnd: {course.NotifyEndDate}");

            // ✅ FIX: Use StartDateTime and EndDateTime for comparisons
            Debug.WriteLine($"StartDateTime > Now: {course.StartDateTime > DateTime.Now}");
            Debug.WriteLine($"EndDateTime > Now: {course.EndDateTime > DateTime.Now}");
            // Cancel any existing notifications for this course first
            await CancelCourseNotificationsAsync(course.Id);

            var results = new List<bool>();

            // Schedule start date notification if enabled
            if (course.NotifyStartDate && course.StartDateTime > DateTime.Now)  // Changed to StartDateTime
            {
                var startNotification = new NotificationRequest
                {
                    NotificationId = GenerateCourseNotificationId(course.Id, isStartDate: true),
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
                    NotificationId = GenerateCourseNotificationId(course.Id, isStartDate: false),
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
            Debug.WriteLine($"Assessment ID: {assessment.Id}");
            Debug.WriteLine($"StartDate: {assessment.StartDate} | StartDateTime: {assessment.StartDateTime}");
            Debug.WriteLine($"EndDate: {assessment.EndDate} | EndDateTime: {assessment.EndDateTime}");
            Debug.WriteLine($"NotifyStart: {assessment.NotifyStartDate} | NotifyEnd: {assessment.NotifyEndDate}");
            Debug.WriteLine($"Current time: {DateTime.Now}");

            // ✅ FIX: Use StartDateTime and EndDateTime for comparisons
            Debug.WriteLine($"StartDateTime > Now: {assessment.StartDateTime > DateTime.Now}");
            Debug.WriteLine($"EndDateTime > Now: {assessment.EndDateTime > DateTime.Now}");

            await CancelAssessmentNotificationsAsync(assessment.Id);

            var results = new List<bool>();

            // ✅ FIX: Compare against StartDateTime instead of StartDate
            if (assessment.NotifyStartDate && assessment.StartDateTime > DateTime.Now)
            {
                Debug.WriteLine("✅ Conditions met for START notification");

                var startNotification = new NotificationRequest
                {
                    NotificationId = GenerateAssessmentNotificationId(assessment.Id, isStartDate: true),
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
                Debug.WriteLine($"Assessment start notification scheduled for {assessment.StartDateTime:MMM dd, yyyy h:mm tt} - Result: {startResult}");
            }
            else
            {
                Debug.WriteLine("❌ START notification skipped - conditions not met");
                Debug.WriteLine($"  - NotifyStartDate: {assessment.NotifyStartDate}");
                Debug.WriteLine($"  - StartDateTime > Now: {assessment.StartDateTime > DateTime.Now}");
            }

            // ✅ FIX: Compare against EndDateTime instead of EndDate
            if (assessment.NotifyEndDate && assessment.EndDateTime > DateTime.Now)
            {
                Debug.WriteLine("✅ Conditions met for END notification");

                var endNotification = new NotificationRequest
                {
                    NotificationId = GenerateAssessmentNotificationId(assessment.Id, isStartDate: false),
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
                Debug.WriteLine($"Assessment end notification scheduled for {assessment.EndDateTime:MMM dd, yyyy h:mm tt} - Result: {endResult}");
            }
            else
            {
                Debug.WriteLine("❌ END notification skipped - conditions not met");
                Debug.WriteLine($"  - NotifyEndDate: {assessment.NotifyEndDate}");
                Debug.WriteLine($"  - EndDateTime > Now: {assessment.EndDateTime > DateTime.Now}");
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
            Debug.WriteLine($"💥 Error scheduling assessment notifications: {ex.Message}");
            return false;
        }
    }
    private async Task CancelCourseNotificationsAsync(string courseId)
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

    private async Task CancelAssessmentNotificationsAsync(string assessmentId)
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

    private int GenerateNotificationId(string entityId, int typeCode)
    {
        // Generate consistent integer ID from string + type code
        return (entityId.GetHashCode() & 0x7FFFFFFF) * 100 + typeCode;
    }

    private int GenerateCourseNotificationId(string courseId, bool isStartDate)
    {
        int typeCode = isStartDate ? 1 : 2;
        return GenerateNotificationId(courseId, typeCode);
    }

    private int GenerateAssessmentNotificationId(string assessmentId, bool isStartDate)
    {
        int typeCode = isStartDate ? 3 : 4;
        return GenerateNotificationId(assessmentId, typeCode);
    }
}