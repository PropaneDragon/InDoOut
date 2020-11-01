using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Executable_Core.Programs;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Despite the name, this is actually uploading to the client, not downloading onto the server.
/// </summary>
namespace InDoOut_Networking.Server.Commands
{
    public class DownloadProgramServerCommand : CommandListener<IServer>
    {
        public IProgramHolder ProgramHolder { get; set; } = null;
        public ILoadedPlugins LoadedPlugins { get; set; } = null;
        public IFunctionBuilder FunctionBuilder { get; set; } = null;

        public DownloadProgramServerCommand(IServer server, IProgramHolder programHolder, ILoadedPlugins loadedPlugins, IFunctionBuilder functionBuilder) : base(server)
        {
            ProgramHolder = programHolder;
            LoadedPlugins = loadedPlugins;
            FunctionBuilder = functionBuilder;
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (ProgramHolder != null && LoadedPlugins != null && FunctionBuilder != null && (message?.Data?.Length ?? 0) == 1)
            {
                var programToFind = message.Data[0];
                if (!string.IsNullOrWhiteSpace(programToFind))
                {
                    var program = ProgramHolder?.Programs?.FirstOrDefault(program => program.Name == programToFind);
                    if (program != null)
                    {
                        using var memoryStream = new MemoryStream();
                        var jsonStorer = new ProgramJsonStorer(FunctionBuilder, LoadedPlugins);
                        var failures = jsonStorer.Save(program, memoryStream);

                        if (failures.Count <= 0)
                        {
                            try
                            {
                                if (memoryStream.CanSeek)
                                {
                                    _ = memoryStream.Seek(0, SeekOrigin.Begin);
                                }

                                using var reader = new StreamReader(memoryStream);
                                var programData = reader.ReadToEnd();

                                return message.CreateResponseMessage(programData);
                            }
                            catch
                            {
                                return message.CreateFailureResponse("Couldn't write out program data.");
                            }
                        }
                        else
                        {
                            return message.CreateFailureResponse("The program couldn't be written due to storage errors.");
                        }
                    }
                    else
                    {
                        return message.CreateFailureResponse($"The program with the name \"{programToFind}\" doesn't exist on the server.");
                    }
                }
                else
                {
                    return message.CreateFailureResponse($"The program name requested was empty.");
                }
            }
            
            return message.CreateFailureResponse($"The request appears to be invalid and can't be accepted by the server.");
        }
    }
}
