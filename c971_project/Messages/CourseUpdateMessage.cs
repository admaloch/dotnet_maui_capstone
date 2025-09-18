using CommunityToolkit.Mvvm.Messaging.Messages;

namespace c971_project.Messages
{
    public class CourseUpdatedMessage : ValueChangedMessage<bool>
    {
        public CourseUpdatedMessage() : base(true) { }
    }
}
