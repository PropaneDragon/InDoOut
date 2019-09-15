using InDoOut_Core.Entities.Functions;
using Q42.HueApi;
using System;
using System.Collections.Generic;

namespace InDoOut_Philips_Hue_Plugins
{
    public class SetLightColourFunction : AbstractApiFunction
    {
        private readonly IOutput _lightChanged, _error;
        private readonly IProperty<string> _lightId;
        private readonly IProperty<double> _fadeSpeedMs, _huePercentage, _saturationPercentage, _brightnessPercentage;

        public override string Description => "Changes the colour of a light and switches it on, given a light ID.";

        public override string Name => "Set light colour";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "light", "white", "bright", "on", "switch", "change", "colour", "color", "hue", "red", "green", "blue", "hsv", "rgb" };

        public SetLightColourFunction()
        {
            _ = CreateInput("Change colour");

            _lightChanged = CreateOutput("Light changed", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));
            _fadeSpeedMs = AddProperty(new Property<double>("Fade speed (ms)", "The speed to fade the light in, in milliseconds.", false, 1000));
            _huePercentage = AddProperty(new Property<double>("Hue colour (%)", "The hue to set the light to as a percentage, where 100% is red, 33% is green and 66% is blue.", false, 100));
            _saturationPercentage = AddProperty(new Property<double>("Saturation (%)", "The saturation to set the light to as a percentage, where 100% full saturation (colourful) and 0% is no saturation (white).", false, 100));
            _brightnessPercentage = AddProperty(new Property<double>("Brightness (%)", "The brightness to set the light to when switched on as a percentage. A percentage less than 0 uses the previous state.", false, 100));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var light = HueHelpers.GetLight(client, _lightId);
                if (light != null)
                {
                    var command = new LightCommand()
                    {
                        Hue = (ushort)Math.Clamp(Math.Round((ushort.MaxValue / 100d) * _huePercentage.FullValue), 0, ushort.MaxValue),
                        Saturation = (byte)Math.Clamp(Math.Round((byte.MaxValue / 100d) * _saturationPercentage.FullValue), 0, byte.MaxValue),
                        Brightness = (byte)Math.Clamp(Math.Round((byte.MaxValue / 100d) * _brightnessPercentage.FullValue), 0, byte.MaxValue),
                        TransitionTime = TimeSpan.FromMilliseconds(_fadeSpeedMs.FullValue),
                        On = true
                    };

                    var results = client.SendCommandAsync(command, new List<string> { light.Id }).Result;
                    if (results.TrueForAll(result => result.Success != null))
                    {
                        return _lightChanged;
                    }
                }
            }

            return _error;
        }
    }
}
