using BlinkStickDotNet;
using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_BlinkStick_Plugins
{
    public class ForEachLightFunction : LoopFunction
    {
        private readonly IOutput _error;
        private readonly IResult _lightId;

        private IEnumerable<string> _cachedLightIds;

        public override string Description => "Loops through all BlinkStick lights connected to the device";

        public override string Name => "For each BlinkStick light";

        public override string Group => "BlinkStick";

        public override string[] Keywords => new[] { "loop", "foreach", "lights", "bulbs" };

        public override IOutput TriggerOnFailure => _error;

        public ForEachLightFunction() : base()
        {
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddResult(new Result("Light ID", "The ID of this light on the bridge."));
        }

        protected override void PreprocessItems()
        {
            var blinkSticks = BlinkStick.FindAll();
            _cachedLightIds = blinkSticks.Select(blinkStick => blinkStick.Meta.Serial);
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            return _cachedLightIds != null && index < _cachedLightIds.Count() ? _lightId.ValueFrom(_cachedLightIds.ElementAt(index)) : false;
        }

        protected override void AllItemsComplete()
        {
            _cachedLightIds = null;
        }
    }
}
