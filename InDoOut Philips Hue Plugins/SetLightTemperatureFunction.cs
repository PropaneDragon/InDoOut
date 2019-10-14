using InDoOut_Core.Entities.Functions;
using Q42.HueApi;
using System;
using System.Collections.Generic;

namespace InDoOut_Philips_Hue_Plugins
{
    public class SetLightTemperatureFunction : AbstractApiFunction
    {
        private readonly IOutput _lightChanged, _error;
        private readonly IProperty<string> _lightId;
        private readonly IProperty<double> _fadeSpeedMs, _temperaturePercentage, _brightnessPercentage;

        public override string Description => "Changes the colour of a light and switches it on, given a light ID.";

        public override string Name => "Set light temperature";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "light", "white", "bright", "on", "switch", "change", "colour", "color", "temperature", "red", "green", "blue", "hsv", "rgb", "kelvins" };

        public override IOutput TriggerOnFailure => _error;

        public SetLightTemperatureFunction()
        {
            _ = CreateInput("Change light temperature");

            _lightChanged = CreateOutput("Light changed", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));
            _fadeSpeedMs = AddProperty(new Property<double>("Fade speed (ms)", "The speed to fade the light in, in milliseconds.", false, 1000));
            _brightnessPercentage = AddProperty(new Property<double>("Brightness (%)", "The brightness to set the light to when switched on as a percentage. A percentage less than 0 uses the previous state.", false, 100));
            _temperaturePercentage = AddProperty(new Property<double>("Colour temperature (%)", "The temperature to set the light to as a percentage, where 100% is 2000K and 0% is 6500K.", false, 100));
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
                        ColorTemperature = 153 + (int)Math.Round(347 * (Math.Clamp(_temperaturePercentage.FullValue, 0, 100) / 100d)),
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
