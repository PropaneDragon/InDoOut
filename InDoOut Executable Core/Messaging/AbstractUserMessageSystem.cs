namespace InDoOut_Executable_Core.Messaging
{
    public abstract class AbstractUserMessageSystem : IAbstractUserMessageSystem
    {
        public abstract void ShowError(string title, string message, string details = null);
        public abstract void ShowWarning(string title, string message, string details = null);
        public abstract void ShowInformation(string title, string message, string details = null);
        public abstract UserResponse? ShowQuestion(string title, string message);
    }
}
