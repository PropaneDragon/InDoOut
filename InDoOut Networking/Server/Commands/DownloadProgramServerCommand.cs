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

        public DownloadProgramServerCommand(IServer server, IProgramHolder programHolder) : base(server)
        {
            ProgramHolder = programHolder;
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (ProgramHolder != null && message.Data.Length == 1)
            {
                var programToFind = message.Data[0];
                if (!string.IsNullOrWhiteSpace(programToFind))
                {
                    var program = ProgramHolder?.Programs?.FirstOrDefault(program => program.Name == programToFind);
                    if (program != null)
                    {
                        using var memoryStream = new MemoryStream();
                        var functionBuilder = new FunctionBuilder();
                        var jsonStorer = new ProgramJsonStorer(functionBuilder, LoadedPlugins.Instance);
                        var failures = jsonStorer.Save(program, memoryStream);

                        if (failures.Count <= 0)
                        {
                            try
                            {
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
                }
            }

            return null;
        }
    }
}
