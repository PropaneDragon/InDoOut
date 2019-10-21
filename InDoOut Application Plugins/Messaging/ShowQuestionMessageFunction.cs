using InDoOut_Core.Entities.Functions;
using InDoOut_Executable_Core.Messaging;

namespace InDoOut_Application_Plugins.Messaging
{
    public class ShowQuestionMessageFunction : Function
    {
        private readonly IOutput _yes, _no, _cancel;
        private readonly IProperty<string> _title, _message;

        public override string Description => "Displays a yes/no question to the user.";

        public override string Name => "Show question message";

        public override string Group => "Messaging";

        public override string[] Keywords => new[] { "?", "??", "popup", "message", "messaging", "window", "info", "information", "yes", "no", "query" };

        public override IOutput TriggerOnFailure => _cancel;

        public ShowQuestionMessageFunction()
        {
            _ = CreateInput("Show question message");

            _yes = CreateOutput("Yes", OutputType.Positive);
            _no = CreateOutput("No", OutputType.Negative);
            _cancel = CreateOutput("Cancel", OutputType.Neutral);

            _title = AddProperty(new Property<string>("Title", "The title to give the message"));
            _message = AddProperty(new Property<string>("Messasge", "The full message to put in the body"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var response = UserMessageSystemHolder.Instance.CurrentUserMessageSystem.ShowQuestion(_title.FullValue, _message.FullValue);

            return response switch
            {
                UserResponse.Yes => _yes,
                UserResponse.No => _no,
                UserResponse.Cancel => _cancel,

                _ => _cancel
            };
        }
    }
}
