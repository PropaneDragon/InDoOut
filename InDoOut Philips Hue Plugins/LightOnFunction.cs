using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Q42.HueApi;
using System;
using System.Collections.Generic;

namespace InDoOut_Philips_Hue_Plugins
{
    public class LightOnFunction : AbstractApiFunction
    {
        private readonly IOutput _lightChanged, _error;
        private readonly IProperty<string> _lightId;
        private readonly IProperty<int> _fadeSpeedMs, _brightnessPercentage;

        public override string Description => "Turns a light on, given a light ID.";

        public override string Name => "Light on";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "light", "white", "bright", "on", "switch", "change" };

        public LightOnFunction()
        {
            _ = CreateInput("Light on");

            _lightChanged = CreateOutput("Light turned on", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));
            _fadeSpeedMs = AddProperty(new Property<int>("Fade speed (ms)", "The speed to fade the light in, in milliseconds.", false, 1000));
            _brightnessPercentage = AddProperty(new Property<int>("Brightness (%)", "The brightness to set the light to when switched on as a percentage. A percentage less than 0 uses the previous state.", false, 100));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = TryGet.ValueOrDefault(() => new LocalHueClient(BridgeIPProperty.FullValue, UserIdProperty.FullValue), null);
            if (client != null)
            {
                var light = TryGet.ValueOrDefault(() => client.GetLightAsync(_lightId.FullValue).Result, null);
                if (light != null)
                {
                    var command = new LightCommand()
                    {
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
