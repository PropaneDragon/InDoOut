using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Q42.HueApi;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ForEachMotionSensor : AbstractForEachApiFunction
    {
        private readonly IResult _sensorId;

        private IEnumerable<string> _cachedSensorIds;

        public override string Description => "Loops through all motion sensors connected to the given bridge";

        public override string Name => "For each motion sensor";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "loop", "foreach", "sensors", "motion", "movement", "ir", "infra red", "radar" };

        public ForEachMotionSensor() : base()
        {
            _sensorId = AddResult(new Result("Sensor ID", "The ID of this sensor on the bridge."));
        }

        protected override void PreprocessItems()
        {
            _cachedSensorIds = null;

            var client = TryGet.ValueOrDefault(() => new LocalHueClient(BridgeIPProperty.FullValue, UserIdProperty.FullValue), null);
            if (client != null)
            {
                _cachedSensorIds = client.GetSensorsAsync().Result.Where(sensor => sensor.Type == "ZLLPresence").Select(sensor => sensor.Id);
            }
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            return _cachedSensorIds != null && index < _cachedSensorIds.Count() ? _sensorId.ValueFrom(_cachedSensorIds.ElementAt(index)) : false;
        }

        protected override void AllItemsComplete()
        {
            _cachedSensorIds = null;
        }
    }
}
