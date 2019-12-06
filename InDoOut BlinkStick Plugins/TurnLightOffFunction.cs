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
                var leds = stick.GetLedCount();
                var colours = new byte[leds * 3];

                for (var led = 0; led < leds; ++led)
                {
                    colours[led] = 0;
                    colours[led + 1] = 0;
                    colours[led + 2] = 0;
                }

                stick.SetColors(0, colours);
                stick.TurnOff();

                return _lightChanged;
            }

            return _error;
        }
    }
}
