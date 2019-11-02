using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Logging;

namespace InDoOut_Debug_Plugins.Logging
{
    public class LogInformationFunction : Function
    {
        private readonly IOutput _logged;
        private readonly IProperty<string> _text;

        public override string Description => "Logs information to the log file.";

        public override string Name => "Log information";

        public override string Group => "Logging";
        public override string[] Keywords => new[] { "debugging", "console", "logger", "file", "disk" };

        public LogInformationFunction()
        {
            _ = CreateInput("Log");

            _logged = CreateOutput("Logged");

            _text = AddProperty(new Property<string>("Text", "The text to log to the log file", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            Log.Instance.Info(_text.FullValue);

            return _logged;
        }
    }
}
