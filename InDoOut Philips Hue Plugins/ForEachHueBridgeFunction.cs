using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Newtonsoft.Json.Linq;
using System;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ForEachHueBridgeFunction : LoopFunction
    {
        private JArray _bridges = null;
        private readonly IResult _bridgeId, _bridgeIp;

        public override string Description => "Loops through all Hue bridges on the current network";

        public override string Name => "For each bridge";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "hue", "philips", "smart home", "light", "bridge", "hub", "meethue" };

        public ForEachHueBridgeFunction()
        {
            _bridgeId = AddResult(new Result("Bridge ID", "The unique ID for this bridge."));
            _bridgeIp = AddResult(new Result("Bridge IP", "The IP address of this bridge on the local network."));
        }

        protected override void PreprocessItems()
        {
            _bridges = JsonFromUrl.Instance.JsonArrayFromUrl(new Uri("https://discovery.meethue.com/"), JsonFromUrl.Method.GET).Result;
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            if (index < (_bridges?.Count ?? 0))
            {
                dynamic bridge = TryGet.ValueOrDefault(() => _bridges[index], null);
                if (bridge != null)
                {
                    _bridgeId.RawValue = TryGet.ValueOrDefault(() => bridge.id);
                    _bridgeIp.RawValue = TryGet.ValueOrDefault(() => bridge.internalipaddress);

                    return !string.IsNullOrEmpty(_bridgeId.RawValue) && !string.IsNullOrEmpty(_bridgeIp.RawValue);
                }
            }

            return false;
        }

        protected override void AllItemsComplete()
        {
            _bridges = null;
        }
    }
}
