using System.Linq;
using InDoOut_Core.Entities.Functions;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetSensorByNameFunction : AbstractApiFunction
    {
        private readonly IOutput _sensorFound, _sensorInvalid;
        private readonly IProperty<string> _sensorName;
        private readonly IResult _sensorId;

        public override string Description => "Finds a sensor ID from an exact name.";

        public override string Name => "Get sensor from exact name";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "sensor", "precise", "id", "search", "find" };

        public GetSensorByNameFunction()
        {
            _ = CreateInput("Find sensor");

            _sensorFound = CreateOutput("Sensor found", OutputType.Positive);
            _sensorInvalid = CreateOutput("Sensor invalid", OutputType.Negative);

            _sensorName = AddProperty(new Property<string>("Sensor name", "The sensor to find", true));

            _sensorId = AddResult(new Result("Sensor ID", "The ID of the found light name"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var foundSensor = HueHelpers.GetSensors(client, _sensorName, false)?.FirstOrDefault();
                if (foundSensor != null)
                {
                    _sensorId.RawValue = foundSensor.Id;

                    return _sensorFound;
                }
            }

            return _sensorInvalid;
        }
    }
}
