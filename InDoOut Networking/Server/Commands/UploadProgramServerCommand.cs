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

namespace InDoOut_Networking.Server.Commands
{
    public class UploadProgramServerCommand : CommandListener<IServer>
    {
        public IProgramHolder ProgramHolder { get; set; } = null;
        public ILoadedPlugins LoadedPlugins { get; set; } = null;
        public IFunctionBuilder FunctionBuilder { get; set; } = null;

        public UploadProgramServerCommand(IServer server, IProgramHolder programHolder, ILoadedPlugins loadedPlugins, IFunctionBuilder functionBuilder) : base(server)
        {
            ProgramHolder = programHolder;
            LoadedPlugins = loadedPlugins;
            FunctionBuilder = functionBuilder;
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (command != null && command.Valid && (command.Data?.Length ?? 0) == 1)
            {
                var programData = command.Data[0];

                if (!string.IsNullOrEmpty(programData))
                {
                    var program = ProgramHolder?.NewProgram();
                    if (FunctionBuilder != null && LoadedPlugins != null && program != null)
                    {
                        try
                        {
                            using var memoryStream = new MemoryStream();
                            var writer = new StreamWriter(memoryStream, leaveOpen: true);

                            writer.Write(programData);
                            writer.Flush();

                            var storer = new ProgramJsonStorer(FunctionBuilder, LoadedPlugins);
                            var failures = storer.Load(program, memoryStream);

                            if (failures.Count == 0 && memoryStream.CanRead)
                            {
                                program.Trigger(null);

                                return command.CreateSuccessResponse();
                            }
                            else
                            {
                                _ = ProgramHolder.RemoveProgram(program);

                                return command.CreateFailureResponse($"The program couldn't be loaded onto the server with the following failures:\n\n{string.Join("\n", failures.Select(failure => $"- {failure}"))}");
                            }
                        }
                        catch
                        {
                            return command.CreateFailureResponse("The program couldn't be read into the server.");
                        }
                    }
                    else
                    {
                        return command.CreateFailureResponse("The server has no way of holding the program to be run. This is an issue with the software itself.");
                    }
                }
                else
                {
                    return command.CreateFailureResponse("The program contained no readable data and can't be constructed.");
                }
            }

            return command.CreateFailureResponse("The program received appeared to be invalid and can't be parsed.");
        }
    }
}
