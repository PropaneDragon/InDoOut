using InDoOut_Core.Entities.Functions;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetSensorPresenceFunction : AbstractApiFunction
    {
        private readonly IOutput _present, _notPresent, _invalidSensor;
        private readonly IProperty<string> _sensorId;

        public override string Description => "Checks whether someone is present on the selected sensor.";

        public override string Name => "Get sensor presence";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "presence", "sensor", "motion", "activated", "triggered" };

        public GetSensorPresenceFunction()
        {
            _ = CreateInput("Check for presence");

            _present = CreateOutput("Someone present", OutputType.Positive);
            _notPresent = CreateOutput("No one present", OutputType.Negative);
            _invalidSensor = CreateOutput("Sensor error", OutputType.Negative);

            _sensorId = AddProperty(new Property<string>("Sensor ID", "The ID for the sensor to check.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var sensor = HueHelpers.GetSensor(client, _sensorId);
                var combinedSensors = HueHelpers.GetCombinedSensors(client, sensor);
                var motionSensor = HueHelpers.GetMotionSensors(combinedSensors)?.FirstOrDefault();

                if (motionSensor != null && motionSensor.State?.Presence != null)
                {
                    return (motionSensor.State.Presence ?? false) ? _present : _notPresent;
                }
            }

            return _invalidSensor;
        }
    }
}
