using InDoOut_Core.Entities.Functions;
using System;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetSensorBrightnessFunction : AbstractApiFunction
    {
        private readonly IOutput _dark, _light, _invalidSensor;
        private readonly IProperty<string> _sensorId;
        private readonly IResult _lux;

        public override string Description => "Checks the current room brightness from a sensor.";

        public override string Name => "Get sensor brightness";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "lux", "daylight", "dark", "night", "day", "sun", "brightness", "light", "room" };

        public override IOutput TriggerOnFailure => _invalidSensor;

        public GetSensorBrightnessFunction()
        {
            _ = CreateInput("Check room brightness");

            _light = CreateOutput("Room is light", OutputType.Neutral);
            _dark = CreateOutput("Room is dark", OutputType.Neutral);
            _invalidSensor = CreateOutput("Sensor error", OutputType.Negative);

            _sensorId = AddProperty(new Property<string>("Sensor ID", "The ID for the sensor to check.", true));

            _lux = AddResult(new Result("Light level (lux)", "The current light level detected by the sensor.", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var sensor = HueHelpers.GetSensor(client, _sensorId);
                var combinedSensors = HueHelpers.GetCombinedSensors(client, sensor);
                var lightSensor = HueHelpers.GetLightSensors(combinedSensors)?.FirstOrDefault();

                if (lightSensor != null && lightSensor.State.LightLevel != null)
                {
                    var lightLevel = lightSensor.State.LightLevel;
                    var normalisedLightLevel = (lightLevel ?? 0) / 10000d;
                    var lux = Math.Round(Math.Pow(10, normalisedLightLevel));

                    if (_lux.ValueFrom(lux))
                    {
                        return (lightSensor?.State?.Dark ?? false) ? _dark : _light;
                    }
                }
            }

            return _invalidSensor;
        }
    }
}
