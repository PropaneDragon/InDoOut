using InDoOut_Core.Entities.Functions;
using InDoOut_Executable_Core.Messaging;

namespace InDoOut_Application_Plugins.Messaging
{
    public class ShowWarningMessageFunction : Function
    {
        private readonly IOutput _closed;
        private readonly IProperty<string> _title, _message;

        public override string Description => "Displays a warning message to the user.";

        public override string Name => "Show warning message";

        public override string Group => "Messaging";

        public override string[] Keywords => new[] { "warn", "popup", "messaging", "window", "display", "message box", "information", "exclamation", "strong" };

        public override IOutput TriggerOnFailure => _closed;

        public ShowWarningMessageFunction()
        {
            _ = CreateInput("Show warning message");

            _closed = CreateOutput("Message closed");

            _title = AddProperty(new Property<string>("Title", "The title to give the message"));
            _message = AddProperty(new Property<string>("Messasge", "The full message to put in the body"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowWarning(_title.FullValue, _message.FullValue);

            return _closed;
        }
    }
}
