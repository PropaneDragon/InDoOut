using InDoOut_Core.Entities.Functions;
using Q42.HueApi;
using System;
using System.Collections.Generic;

namespace InDoOut_Philips_Hue_Plugins
{
    public class LightOffFunction : AbstractApiFunction
    {
        private readonly IOutput _lightChanged, _error;
        private readonly IProperty<string> _lightId;
        private readonly IProperty<double> _fadeSpeedMs;

        public override string Description => "Turns a light off, given a light ID.";

        public override string Name => "Turn light off";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "dark", "black", "off", "switch", "change" };

        public override IOutput TriggerOnFailure => _error;

        public LightOffFunction()
        {
            _ = CreateInput("Turn light off");

            _lightChanged = CreateOutput("Light turned off", OutputType.Positive);
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
                        TransitionTime = TimeSpan.FromMilliseconds(_fadeSpeedMs.FullValue),
                        On = false
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
