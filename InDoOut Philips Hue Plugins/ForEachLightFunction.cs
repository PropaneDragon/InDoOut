using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ForEachLightFunction : AbstractForEachApiFunction
    {
        private readonly IProperty<string> _name;
        private readonly IResult _lightId;

        private IEnumerable<string> _cachedLightIds;

        public override string Description => "Loops through all lights connected to the given bridge";

        public override string Name => "For each light";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "loop", "foreach", "lights", "bulbs" };

        public ForEachLightFunction() : base()
        {
            _name = AddProperty(new Property<string>("Name", "Leave blank for all lights. Searches for lights that contain the given name.", false, null));

            _lightId = AddResult(new Result("Light ID", "The ID of this light on the bridge."));
        }

        protected override void PreprocessItems()
        {
            _cachedLightIds = null;

            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                _cachedLightIds = HueHelpers.GetLights(client, _name, true).Select(light => light.Id);
            }
        }

        protected override bool PopulateItemDataForIndex(int index) => _cachedLightIds != null && index < _cachedLightIds.Count() ? _lightId.ValueFrom(_cachedLightIds.ElementAt(index)) : false;

        protected override void AllItemsComplete() => _cachedLightIds = null;
    }
}
