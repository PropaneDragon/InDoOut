using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Client.Commands
{
    public class DownloadProgramClientCommand : Command<IClient>
    {
        public ILoadedPlugins LoadedPlugins { get; set; } = null;
        public IFunctionBuilder FunctionBuilder { get; set; } = null;

        public DownloadProgramClientCommand(IClient client, ILoadedPlugins loadedPlugins, IFunctionBuilder functionBuilder) : base(client)
        {
            LoadedPlugins = loadedPlugins;
            FunctionBuilder = functionBuilder;
        }

        public async Task<string> RequestDataForProgram(string programName, CancellationToken cancellationToken)
        {
            var response = await SendMessageAwaitResponse(cancellationToken, programName);

            return (response?.Data?.Length ?? 0) == 1 ? response.Data[0] : null;
        }

        public async Task<bool> RequestProgram(string programName, IProgram programToLoadInto, CancellationToken cancellationToken)
        {
            var data = await RequestDataForProgram(programName, cancellationToken);
            if (!string.IsNullOrEmpty(data))
            {
                using var memoryStream = new MemoryStream();

                try
                {
                    using var streamWriter = new StreamWriter(memoryStream, leaveOpen: true);
                    streamWriter.Write(data);
                    streamWriter.Flush();
                }
                catch { }

                var functionBuilder = new FunctionBuilder();
                var jsonStorer = new ProgramJsonStorer(FunctionBuilder, LoadedPlugins);
                var failures = jsonStorer.Load(programToLoadInto, memoryStream);

                return failures.Count == 0;
            }

            return false;
        }
    }
}
