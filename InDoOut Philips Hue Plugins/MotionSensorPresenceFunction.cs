using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Q42.HueApi;

namespace InDoOut_Philips_Hue_Plugins
{
    public class MotionSensorPresenceFunction : AbstractApiFunction
    {
        private readonly IOutput _present, _notPresent, _invalidSensor;
        private readonly IProperty<string> _sensorId;

        public override string Description => "Checks whether someone is present on the selected sensor.";

        public override string Name => "Motion sensor presence";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "presence", "sensor", "motion", "activated", "triggered" };

        public MotionSensorPresenceFunction()
        {
            _ = CreateInput("Check presence");

            _present = CreateOutput("Someone present", OutputType.Positive);
            _notPresent = CreateOutput("No one present", OutputType.Negative);
            _invalidSensor = CreateOutput("Sensor error", OutputType.Negative);

            _sensorId = AddProperty(new Property<string>("Sensor ID", "The ID for the sensor to check.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = TryGet.ValueOrDefault(() => new LocalHueClient(BridgeIPProperty.FullValue, UserIdProperty.FullValue), null);
            if (client != null)
            {
                var sensor = TryGet.ValueOrDefault(() => client.GetSensorAsync(_sensorId.FullValue).Result, null);
                if (sensor != null && sensor.State.Presence != null)
                {
                    return (sensor.State.Presence ?? false) ? _present : _notPresent;
                }
            }

            return _invalidSensor;
        }
    }
}
