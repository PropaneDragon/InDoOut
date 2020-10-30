using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Location;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace InDoOut_Application_Plugins.Self
{
    public class RunIDOProgramFunction : SelfRunnerFunction
    {
        private static readonly int MAX_WAIT_COUNT = 100;

        private readonly IOutput _completed, _failed;
        private readonly IProperty<string> _programPath;
        private readonly IProperty<bool> _reloadEachTime;
        private readonly IResult _result;
        private readonly List<IProperty<string>> _inputValues = new List<IProperty<string>>();

        private IProgram _currentProgram = null;

        public ILoadedPlugins PluginsToUse { get; set; } = LoadedPlugins.Instance;

        public override IOutput TriggerOnFailure => _failed;

        public override string Description => "Runs an IDO program internally, allowing for programs to be used as self-contained functions.";

        public override string Name => "Run IDO program";

        public override string Group => "Self";

        public override string[] Keywords => new[] { "function", "run", "runner", "selfaware", "aware" };

        public override IProgram LoadedProgram => _currentProgram;

        public RunIDOProgramFunction()
        {
            _ = CreateInput("Start");

            _completed = CreateOutput("Completed", OutputType.Positive);
            _failed = CreateOutput("Failed", OutputType.Negative);

            _programPath = AddProperty(new Property<string>("Path", "The full path to the IDO program.", true));
            _reloadEachTime = AddProperty(new Property<bool>("Reload each time", "When this is true, it reads the program from the disk every time it is called. When false, it will only read it once.", false, false));

            _result = AddResult(new Result("Result", "The result value from the program.", "0"));

            for (var count = 1; count <= StartFunction.TOTAL_OUTPUTS; ++count)
            {
                _inputValues.Add(AddProperty(new Property<string>($"Value {count}", $"Value number {count} to pass into the program.")));
            }
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var finalPath = ConstructProgramPathFromLocations(_programPath.FullValue);

            if (!string.IsNullOrEmpty(finalPath) && File.Exists(finalPath))
            {
                var builder = new FunctionBuilder();

                if (PluginsToUse != null)
                {
                    if (_reloadEachTime.FullValue || _currentProgram == null)
                    {
                        try
                        {
                            using var fileStream = new FileStream(finalPath, FileMode.Open);

                            var loadedProgram = new Program(_inputValues.Select(value => value.FullValue).ToArray());
                            var loader = new ProgramJsonStorer(builder, PluginsToUse, fileStream);
                            var loadResult = loader.Load(loadedProgram);

                            if (loadResult.Count == 0)
                            {
                                _currentProgram = loadedProgram;
                            }
                        }
                        catch { }
                    }

                    if (_currentProgram != null && _currentProgram.StartFunctions.Count > 0)
                    {
                        _currentProgram.Trigger(this);

                        var waitCount = 0;
                        while (waitCount < MAX_WAIT_COUNT)
                        {
                            if (StopRequested && !_currentProgram.Stopping)
                            {
                                _currentProgram.Stop();
                            }

                            if (_currentProgram.Running || _currentProgram.Stopping)
                            {
                                waitCount = 0;
                            }
                            else
                            {
                                ++waitCount;
                            }

                            Thread.Sleep(TimeSpan.FromMilliseconds(10));
                        }

                        _result.RawValue = _currentProgram?.ReturnCode ?? "0";

                        _currentProgram.Stop();
                        _currentProgram = null;

                        builder = null;

                        return _completed;
                    }
                }
            }

            return _failed;
        }

        private string ConstructProgramPathFromLocations(string path)
        {
            if (!string.IsNullOrEmpty(path) && !File.Exists(path) && !Path.IsPathFullyQualified(path))
            {
                var fileName = Path.GetFileName(path);
                var sanitisedPath = path.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

                if (!string.IsNullOrEmpty(fileName))
                {
                    var pathsToTry = new[]
                    {
                        "",
                        FormatPath(StandardLocations.Instance.GetPathTo(Location.ApplicationDirectory)),
                        FormatPath(Path.GetDirectoryName(StandardLocations.Instance.GetPathTo(Location.SaveFile)))
                    };

                    foreach (var pathToTry in pathsToTry)
                    {
                        if (pathToTry != null)
                        {
                            var potentialPaths = new[]
                            {
                                pathToTry + fileName,
                                pathToTry + sanitisedPath
                            };

                            foreach (var potentialPath in potentialPaths)
                            {
                                if (!string.IsNullOrEmpty(potentialPath) && File.Exists(potentialPath))
                                {
                                    return potentialPath;
                                }
                            }
                        }
                    }
                }
            }

            return path;
        }
        
        private string FormatPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                path = Path.TrimEndingDirectorySeparator(path);
                path += Path.DirectorySeparatorChar;
            }

            return path;
        }
    }
}
