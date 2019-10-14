using InDoOut_Core.Entities.Functions;

namespace InDoOut_Philips_Hue_Plugins
{
    public class IsLightOnFunction : AbstractApiFunction
    {
        private readonly IOutput _lightOn, _lightOff, _error;
        private readonly IProperty<string> _lightId;

        public override string Description => "Checks whether a certain light is on.";

        public override string Name => "Is light on";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "light", "white", "bright", "on", "off", "status" };

        public override IOutput TriggerOnFailure => _error;

        public IsLightOnFunction()
        {
            _ = CreateInput("Check light");

            _lightOn = CreateOutput("Light is on", OutputType.Positive);
            _lightOff = CreateOutput("Light is off", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to be checked.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var light = HueHelpers.GetLight(client, _lightId);
                if (light != null)
                {
                    return (light?.State?.On ?? false) ? _lightOn : _lightOff;
                }
            }

            return _error;
        }
    }
}
