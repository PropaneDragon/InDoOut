using System.Linq;
using InDoOut_Core.Entities.Functions;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetLightByNameFunction : AbstractApiFunction
    {
        private readonly IOutput _lightFound, _lightInvalid;
        private readonly IProperty<string> _lightName, _groupId;
        private readonly IResult _lightId;

        public override string Description => "Finds a light ID from an exact name.";

        public override string Name => "Get light from exact name";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "precise", "id", "search", "find" };

        public GetLightByNameFunction()
        {
            _ = CreateInput("Find light");

            _lightFound = CreateOutput("Light found", OutputType.Positive);
            _lightInvalid = CreateOutput("Light invalid", OutputType.Negative);

            _lightName = AddProperty(new Property<string>("Light name", "The name to find", true));
            _groupId = AddProperty(new Property<string>("Group ID", "The group to search for a light in. Leave empty for all groups.", false));

            _lightId = AddResult(new Result("Light ID", "The ID of the found light name"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var foundGroup = HueHelpers.GetGroup(client, _groupId);
                var foundLight = HueHelpers.GetLights(client, foundGroup, _lightName, false)?.FirstOrDefault();

                if (foundLight != null)
                {
                    _lightId.RawValue = foundLight.Id;

                    return _lightFound;
                }
            }

            return _lightInvalid;
        }
    }
}
