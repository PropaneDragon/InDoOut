using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Miscellaneous
{
    public class RelayFunction : Function
    {
        private IOutput _output;

        public override string Description => "Relays an input straight to an output. Keeps cables tidy.";

        public override string Name => "Relay";

        public override string Group => "Miscellaneous";

        public override string[] Keywords => new[] { "relay", "cable", "wire", "tidy", "container", "passthrough", "tie", "wrap" };

        public RelayFunction()
        {
            _ = CreateInput();

            _output = CreateOutput();
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            return _output;
        }
    }
}
