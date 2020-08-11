namespace InDoOut_Executable_Core.Messaging
{
    public enum UserResponse
    {
        Yes,
        No,
        Cancel,
        Ok
    }

    public interface IAbstractUserMessageSystem
    {
        void ShowError(string title, string message, string details = null);
        void ShowWarning(string title, string message, string details = null);
        void ShowInformation(string title, string message, string details = null);
        UserResponse? ShowQuestion(string title, string message);
    }
}