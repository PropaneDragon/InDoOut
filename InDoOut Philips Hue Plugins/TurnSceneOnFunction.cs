using InDoOut_Core.Entities.Functions;
using Q42.HueApi;

namespace InDoOut_Philips_Hue_Plugins
{
    public class TurnSceneOnFunction : AbstractApiFunction
    {
        private readonly IOutput _sceneChanged, _error;
        private readonly IProperty<string> _sceneId;

        public override string Description => "Turns a scene on, given a scene ID.";

        public override string Name => "Turn scene on";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "light", "state", "scene", "white", "bright", "on", "switch", "change" };

        public override IOutput TriggerOnFailure => _error;

        public TurnSceneOnFunction()
        {
            _ = CreateInput("Turn scene on");

            _sceneChanged = CreateOutput("Scene turned on", OutputType.Positive);
            _error = CreateOutput("Scene error", OutputType.Negative);

            _sceneId = AddProperty(new Property<string>("Scene ID", "The ID for the scene to control.", true));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var client = HueHelpers.GetClient(this);
            if (client != null)
            {
                var scene = HueHelpers.GetScene(client, _sceneId);
                if (scene != null)
                {
                    var command = new SceneCommand(scene.Id);

                    var results = client.SendGroupCommandAsync(command).Result;
                    if (results.TrueForAll(result => result.Success != null))
                    {
                        return _sceneChanged;
                    }
                }
            }

            return _error;
        }
    }
}
