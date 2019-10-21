using InDoOut_Core.Entities.Functions;
using InDoOut_Executable_Core.Messaging;

namespace InDoOut_Application_Plugins.Messaging
{
    public class ShowErrorMessageFunction : Function
    {
        private readonly IOutput _closed;
        private readonly IProperty<string> _title, _message;

        public override string Description => "Displays an error message to the user.";

        public override string Name => "Show error message";

        public override string Group => "Messaging";

        public override string[] Keywords => new[] { "exclamation", "popup", "messaging", "window", "display", "message box", "information", "critical" };

        public override IOutput TriggerOnFailure => _closed;

        public ShowErrorMessageFunction()
        {
            _ = CreateInput("Show error message");

            _closed = CreateOutput("Message closed");

            _title = AddProperty(new Property<string>("Title", "The title to give the message"));
            _message = AddProperty(new Property<string>("Messasge", "The full message to put in the body"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowError(_title.FullValue, _message.FullValue);

            return _closed;
        }
    }
}
