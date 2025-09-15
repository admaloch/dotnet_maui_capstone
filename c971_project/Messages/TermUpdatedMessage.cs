using CommunityToolkit.Mvvm.Messaging.Messages;

namespace c971_project.Messages
{
    public class TermUpdatedMessage : ValueChangedMessage<bool>
    {
        public TermUpdatedMessage() : base(true) { }
    }
}
