using CommunityToolkit.Mvvm.Messaging.Messages;

namespace c971_project.Messages
{
    public class NoteUpdatedMessage : ValueChangedMessage<bool>
    {
        public NoteUpdatedMessage() : base(true) { }
    }
}
