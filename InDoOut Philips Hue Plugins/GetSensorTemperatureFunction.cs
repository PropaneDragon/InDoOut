using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Q42.HueApi;
using Q42.HueApi.Models;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetSensorTemperatureFunction : AbstractApiFunction
    {
        private readonly IOutput _gotTemperature, _invalidSensor;
        private readonly IProperty<string> _sensorId;
        private readonly IResult _temperature;

        public override string Description => "Checks the current room temperature detected by a sensor.";

        public override string Name => "Get sensor temperature";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "heat", "temperature", "celsius", "farenheit", "cold", "hot", "warm", "chilly", "thermostat" };

        public override IOutput TriggerOnFailure => _invalidSensor;

        public GetSensorTemperatureFunction()
        {
            _ = CreateInput("Check temperature");

            _gotTemperature = CreateOutput("Temperature obtained", OutputType.Neutral);
            _invalidSensor = CreateOutput("Sensor error", OutputType.Negative);

            _sensorId = AddProperty(new Property<string>("Sensor ID", "The ID for the sensor to check.", true));

            _temperature = AddResult(new Result("Temperature (celsius)", "The current temperature detected by the sensor.", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var sensor = HueHelpers.GetSensor(client, _sensorId);
                var combinedSensors = HueHelpers.GetCombinedSensors(client, sensor);
                var temperatureSensor = HueHelpers.GetTemperatureSensors(combinedSensors)?.FirstOrDefault();

                if (temperatureSensor != null && temperatureSensor.State?.Temperature != null)
                {
                    var temperature = temperatureSensor.State.Temperature ?? 0;
                    var celsius = temperature / 100d;

                    if (_temperature.ValueFrom(celsius))
                    {
                        return _gotTemperature;
                    }
                }
            }

            return _invalidSensor;
        }
    }
}
