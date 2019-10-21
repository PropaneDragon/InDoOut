using InDoOut_Core.Logging;

namespace InDoOut_Executable_Core.Messaging
{
    public class NullUserMessageSystem : AbstractUserMessageSystem
    {
        public override void ShowError(string title, string message)
        {
            Log.Instance.Warning("Null message system in use for ShowError");
        }

        public override void ShowWarning(string title, string message)
        {
            Log.Instance.Warning("Null message system in use for ShowWarning");
        }

        public override void ShowInformation(string title, string message)
        {
            Log.Instance.Warning("Null message system in use for ShowInformation");
        }

        public override UserResponse? ShowQuestion(string title, string message)
        {
            Log.Instance.Warning("Null message system in use for ShowQuestion");

            return null;
        }
    }
}
