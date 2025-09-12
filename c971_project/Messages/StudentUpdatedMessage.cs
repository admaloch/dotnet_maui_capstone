using CommunityToolkit.Mvvm.Messaging.Messages;

namespace c971_project.Messages
{
    public class StudentUpdatedMessage : ValueChangedMessage<bool>
    {
        public StudentUpdatedMessage() : base(true) { }
    }
}
