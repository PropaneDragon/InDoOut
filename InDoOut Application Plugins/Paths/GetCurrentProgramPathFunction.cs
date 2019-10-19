using InDoOut_Core.Entities.Functions;
using InDoOut_Executable_Core.Location;

namespace InDoOut_Application_Plugins.Paths
{
    public class GetCurrentProgramPathFunction : Function
    {
        private readonly IOutput _output, _noPath;
        private readonly IResult _programPath;

        public override string Description => "Gets the full path to the current program.";

        public override string Name => "Get current program path";

        public override string Group => "Paths";

        public override string[] Keywords => new[] { "running", "this", "active" };

        public override IOutput TriggerOnFailure => _noPath;

        public GetCurrentProgramPathFunction()
        {
            _ = CreateInput("Get path");

            _output = CreateOutput("Path retrieved", OutputType.Positive);
            _noPath = CreateOutput("Path unavailable", OutputType.Negative);

            _programPath = AddResult(new Result("Program path", "The full path to the currently running program."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (StandardLocations.Instance.IsPathSet(Location.SaveFile))
            {
                var savePath = StandardLocations.Instance.GetPathTo(Location.SaveFile);
                if (!string.IsNullOrEmpty(savePath) && _programPath.ValueFrom(savePath))
                {
                    return _output;
                }
            }

            return _noPath;
        }
    }
}
