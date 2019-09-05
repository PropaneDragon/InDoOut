using InDoOut_Core.Entities.Functions;
using Q42.HueApi;
using Q42.HueApi.Models.Bridge;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ForEachHueBridgeFunction : LoopFunction
    {
        private IEnumerable<LocatedBridge> _bridges;
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
            var bridgeLocator = new HttpBridgeLocator();
            _bridges = bridgeLocator.LocateBridgesAsync(TimeSpan.FromSeconds(5)).Result;
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            if (index < (_bridges?.Count() ?? 0))
            {
                var bridge = _bridges.ElementAt(index);

                return _bridgeId.ValueFrom(bridge?.BridgeId ?? "") && _bridgeIp.ValueFrom(bridge?.IpAddress ?? "");
            }

            return false;
        }

        protected override void AllItemsComplete()
        {
            _bridges = null;
        }
    }
}
