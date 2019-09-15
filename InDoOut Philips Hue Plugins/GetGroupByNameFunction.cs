using System.Linq;
using InDoOut_Core.Entities.Functions;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetGroupByNameFunction : AbstractApiFunction
    {
        private readonly IOutput _groupFound, _groupInvalid;
        private readonly IProperty<string> _groupName;
        private readonly IResult _groupId;

        public override string Description => "Finds a light group ID from an exact name.";

        public override string Name => "Get group from exact name";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "group", "precise", "id", "search", "find" };

        public GetGroupByNameFunction()
        {
            _ = CreateInput("Find group");

            _groupFound = CreateOutput("Group found", OutputType.Positive);
            _groupInvalid = CreateOutput("Group invalid", OutputType.Negative);

            _groupName = AddProperty(new Property<string>("Group name", "The name to find", true));

            _groupId = AddResult(new Result("Group ID", "The ID of the found group name"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var matchedGroup = HueHelpers.GetGroups(client, _groupName, false).FirstOrDefault();
                if (matchedGroup != null)
                {
                    _groupId.RawValue = matchedGroup.Id;

                    return _groupFound;
                }
            }

            return _groupInvalid;
        }
    }
}
