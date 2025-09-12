using CommunityToolkit.Mvvm.Messaging.Messages;

namespace c971_project.Messages
{
    public class TermUpdatedMessage : ValueChangedMessage<int>
    {
        public TermUpdatedMessage(int termId = 0) : base(termId) { }
    }
}
