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
        void ShowError(string title, string message);
        void ShowWarning(string title, string message);
        void ShowInformation(string title, string message);
        UserResponse? ShowQuestion(string title, string message);
    }
}