using InDoOut_Core.Entities.Functions;
using System;
using System.Linq;
using System.Threading;

namespace InDoOut_Philips_Hue_Plugins
{
    public class WaitForSensorPresenceFunction : AbstractApiFunction
    {
        private readonly IOutput _present, _invalidSensor;
        private readonly IProperty<string> _sensorId;
        private readonly IProperty<int> _checkInterval;

        private DateTime _lastUpdated = DateTime.MinValue;

        public override string Description => "Waits for presence on the given sensor, then activates the output.";

        public override string Name => "Wait for sensor presence";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "presence", "sensor", "motion", "activated", "triggered" };

        public override IOutput TriggerOnFailure => _invalidSensor;

        public WaitForSensorPresenceFunction()
        {
            _ = CreateInput("Wait for presence");

            _present = CreateOutput("Presence detected", OutputType.Positive);
            _invalidSensor = CreateOutput("Invalid sensor", OutputType.Negative);

            _sensorId = AddProperty(new Property<string>("Sensor ID", "The ID for the sensor to check.", true));
            _checkInterval = AddProperty(new Property<int>("Check interval (milliseconds)", "The interval in milliseconds to check for sensor presence."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            while (true)
            {
                var client = HueHelpers.GetClient(this);
                if (client != null)
                {
                    var sensor = HueHelpers.GetSensor(client, _sensorId);
                    var combinedSensors = HueHelpers.GetCombinedSensors(client, sensor);
                    var motionSensor = HueHelpers.GetMotionSensors(combinedSensors)?.FirstOrDefault();

                    if (motionSensor != null && motionSensor.State?.Presence != null)
                    {
                        if (motionSensor.State.Lastupdated == null || motionSensor.State.Lastupdated != _lastUpdated)
                        {
                            _lastUpdated = motionSensor.State.Lastupdated ?? DateTime.MinValue;

                            if (motionSensor.State.Presence ?? false)
                            {
                                return _present;
                            }
                        }
                    }
                    else
                    {
                        return _invalidSensor;
                    }
                }
                else
                {
                    return _invalidSensor;
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(_checkInterval.FullValue));
            }
        }
    }
}
