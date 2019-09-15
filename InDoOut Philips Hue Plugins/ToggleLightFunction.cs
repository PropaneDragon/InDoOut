using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Q42.HueApi;
using System;
using System.Collections.Generic;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ToggleLightFunction : AbstractApiFunction
    {
        private readonly IOutput _lightToggled, _error;
        private readonly IProperty<string> _lightId;
        private readonly IProperty<double> _fadeSpeedMs;

        public override string Description => "Toggles the light on and off state of a given light ID.";

        public override string Name => "Toggle light on/off";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "light", "white", "bright", "on", "off", "dark", "black", "switch", "change", "toggle" };

        public ToggleLightFunction()
        {
            _ = CreateInput("Toggle light");

            _lightToggled = CreateOutput("Light toggled", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));
            _fadeSpeedMs = AddProperty(new Property<double>("Fade speed (ms)", "The speed to fade the light out, in milliseconds.", false, 1000));
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
                        On = !light.State.On,
                        TransitionTime = TimeSpan.FromMilliseconds(_fadeSpeedMs.FullValue)
                    };

                    var results = client.SendCommandAsync(command, new List<string> { light.Id }).Result;
                    if (results.TrueForAll(result => result.Success != null))
                    {
                        return _lightToggled;
                    }
                }
            }

            return _error;
        }
    }
}
