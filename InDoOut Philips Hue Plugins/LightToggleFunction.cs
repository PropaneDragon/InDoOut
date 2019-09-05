using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Philips_Hue_Plugins
{
    public class LightToggleFunction : AbstractApiFunction
    {
        public override string Description => "Toggles the state of a light, given a light ID.";

        public override string Name => "Light toggle";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "dark", "light", "black", "white", "bright", "off", "on", "switch", "change" };

        public LightToggleFunction()
        {
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            throw new NotImplementedException();
        }
    }
}
