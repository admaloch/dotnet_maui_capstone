using CommunityToolkit.Mvvm.Messaging.Messages;

namespace c971_project.Messages
{
    public class AssessmentUpdatedMessage : ValueChangedMessage<bool>
    {
        public AssessmentUpdatedMessage() : base(true) { }
    }
}
