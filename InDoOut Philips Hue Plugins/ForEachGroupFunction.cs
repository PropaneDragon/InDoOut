using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ForEachGroupFunction : AbstractForEachApiFunction
    {
        private readonly IProperty<string> _groupName;
        private readonly IResult _groupId;

        private IEnumerable<string> _cachedGroupIds = null;

        public override string Description => "Finds a light group ID from a partial name.";

        public override string Name => "For each group";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "group", "partial", "contains", "within", "id", "search", "find" };

        public ForEachGroupFunction()
        {
            _groupName = AddProperty(new Property<string>("Group name", "The name or part of name to find", true));

            _groupId = AddResult(new Result("Group ID", "The ID of the found group name"));
        }

        protected override void PreprocessItems()
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                _cachedGroupIds = HueHelpers.GetGroups(client, _groupName, true)?.Select(group => group.Id);
            }
        }

        protected override bool PopulateItemDataForIndex(int index) => _cachedGroupIds != null && index >= 0 && index < _cachedGroupIds.Count() && _groupId.ValueFrom(_cachedGroupIds.ElementAt(index));

        protected override void AllItemsComplete() => _cachedGroupIds = null;
    }
}
