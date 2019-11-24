using BlinkStickDotNet;
using InDoOut_Core.Entities.Functions;

namespace InDoOut_BlinkStick_Plugins
{
    public class TurnLightOffFunction : Function
    {
        private readonly IOutput _lightChanged, _error;
        private readonly IProperty<string> _lightId;

        public override string Description => "Turns a light off, given a light ID.";

        public override string Name => "Turn BlinkStick light off";

        public override string Group => "BlinkStick";

        public override string[] Keywords => new[] { "light", "state", "dark", "black", "off", "switch", "change" };

        public override IOutput TriggerOnFailure => _error;

        public TurnLightOffFunction()
        {
            _ = CreateInput("Turn light off");

            _lightChanged = CreateOutput("Light turned off", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            using var stick = BlinkStick.FindBySerial(_lightId.FullValue);
            if (stick != null && stick.OpenDevice())
            {
                stick.TurnOff();

                return _lightChanged;
            }

            return _error;
        }
    }
}
