using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ForEachSensorFunction : AbstractForEachApiFunction
    {
        private readonly IProperty<string> _name;
        private readonly IResult _sensorId;

        private IEnumerable<string> _cachedSensorIds;

        public override string Description => "Loops through all sensors connected to the given bridge";

        public override string Name => "For each sensor";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "loop", "foreach", "sensors", "motion", "movement", "ir", "infra red", "radar", "temperature", "humidity", "light" };

        public ForEachSensorFunction() : base()
        {
            _name = AddProperty(new Property<string>("Sensor name", "Leave blank to get all sensors. Searches for sensor names that contain the given name.", false));

            _sensorId = AddResult(new Result("Sensor ID", "The ID of this sensor on the bridge."));
        }

        protected override void PreprocessItems()
        {
            _cachedSensorIds = null;

            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                _cachedSensorIds = HueHelpers.GetSensors(client, _name, true)?.Select(sensor => sensor.Id);
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
