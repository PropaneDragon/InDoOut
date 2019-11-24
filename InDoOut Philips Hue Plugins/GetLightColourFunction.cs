using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetLightColourFunction : AbstractApiFunction
    {
        private readonly IOutput _retrieved, _error;
        private readonly IProperty<string> _lightId;
        private readonly IResult _huePercentage, _saturationPercentage, _brightnessPercentage;

        public override string Description => "Gets the colour of a light, given a light ID.";

        public override string Name => "Get light colour";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "light", "white", "bright", "on", "colour", "color", "hue", "red", "green", "blue", "hsv", "rgb" };

        public override IOutput TriggerOnFailure => _error;

        public GetLightColourFunction()
        {
            _ = CreateInput("Retrieve colour");

            _retrieved = CreateOutput("Colour retrieved", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));
            _huePercentage = AddResult(new Result("Hue colour (%)", "The current hue of the light as a percentage, where 100% is red, 33% is green and 66% is blue.", "0"));
            _saturationPercentage = AddResult(new Result("Saturation (%)", "The saturation of the light as a percentage, where 100% full saturation (colourful) and 0% is no saturation (white).", "0"));
            _brightnessPercentage = AddResult(new Result("Brightness (%)", "The brightness of the light as a percentage.", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var light = HueHelpers.GetLight(client, _lightId);
                if (light?.State != null)
                {
                    var state = light.State;

                    _ = _huePercentage.ValueFrom(Math.Clamp(((state.Hue ?? 0) / (double)ushort.MaxValue) * 100d, 0d, 100d));
                    _ = _saturationPercentage.ValueFrom(Math.Clamp(((state.Saturation ?? 0) / (double)byte.MaxValue) * 100d, 0d, 100d));
                    _ = _brightnessPercentage.ValueFrom(Math.Clamp((state.Brightness / (double)byte.MaxValue) * 100d, 0d, 100d));

                    return _retrieved;
                }
            }

            return _error;
        }
    }
}
