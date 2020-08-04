using System.Linq;
using InDoOut_Core.Entities.Functions;

namespace InDoOut_Philips_Hue_Plugins
{
    public class GetSceneByNameFunction : AbstractApiFunction
    {
        private readonly IOutput _sceneFound, _sceneInvalid;
        private readonly IProperty<string> _sceneName, _groupId;
        private readonly IResult _sceneId;

        public override string Description => "Finds a scene ID from an exact name.";

        public override string Name => "Get scene from exact name";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "scene", "precise", "id", "search", "find" };

        public override IOutput TriggerOnFailure => _sceneInvalid;

        public GetSceneByNameFunction()
        {
            _ = CreateInput("Find scene");

            _sceneFound = CreateOutput("Scene found", OutputType.Positive);
            _sceneInvalid = CreateOutput("Scene invalid", OutputType.Negative);

            _sceneName = AddProperty(new Property<string>("Scene name", "The name to find", true));
            _groupId = AddProperty(new Property<string>("Group ID", "The group to search for a scene in. Leave empty for all groups.", false));

            _sceneId = AddResult(new Result("Scene ID", "The ID of the found scene name"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var foundGroup = HueHelpers.GetGroup(client, _groupId);
                var foundScene = HueHelpers.GetScenes(client, foundGroup, _sceneName, false)?.FirstOrDefault();

                if (foundScene != null)
                {
                    _sceneId.RawValue = foundScene.Id;

                    return _sceneFound;
                }
            }

            return _sceneInvalid;
        }
    }
}
