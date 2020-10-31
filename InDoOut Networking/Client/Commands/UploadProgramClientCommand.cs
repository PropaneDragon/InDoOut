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
    public class UploadProgramClientCommand : Command<IClient>
    {
        public ILoadedPlugins LoadedPlugins { get; set; } = null;
        public IFunctionBuilder FunctionBuilder { get; set; } = null;

        public UploadProgramClientCommand(IClient client, ILoadedPlugins loadedPlugins, IFunctionBuilder functionBuilder) : base(client)
        {
            LoadedPlugins = loadedPlugins;
            FunctionBuilder = functionBuilder;
        }

        public async Task<bool> SendProgram(IProgram program, CancellationToken cancellationToken)
        {
            if (FunctionBuilder != null && LoadedPlugins != null && program != null && !string.IsNullOrEmpty(program.Name))
            {
                var programData = "";

                try
                {
                    using var memoryStream = new MemoryStream();

                    var storer = new ProgramJsonStorer(FunctionBuilder, LoadedPlugins);
                    var failures = storer.Save(program, memoryStream);

                    if (failures.Count == 0 && memoryStream.CanRead)
                    {
                        if (memoryStream.CanSeek)
                        {
                            _ = memoryStream.Seek(0, SeekOrigin.Begin);
                        }

                        using var reader = new StreamReader(memoryStream, leaveOpen: true);

                        programData = reader.ReadToEnd();
                    }
                }
                catch
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(programData))
                {
                    return await SendProgram(programData, cancellationToken);
                }
            }

            return false;
        }

        public async Task<bool> SendProgram(string programData, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(programData))
            {
                var response = await SendMessageAwaitResponse(cancellationToken, programData);

                return response != null && response.Valid && response.SuccessMessage != null;
            }

            return false;
        }
    }
}
