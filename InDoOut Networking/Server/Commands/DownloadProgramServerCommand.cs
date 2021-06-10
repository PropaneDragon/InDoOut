using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Shared.Entities;
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

            if (ProgramHolder != null && (message?.Data?.Length ?? 0) == 1)
            {
                var programToFind = message.Data[0];
                if (!string.IsNullOrWhiteSpace(programToFind))
                {
                    NetworkEntity?.EntityLog?.Info(CommandName, ": Finding program - \"", programToFind, "\"...");

                    var program = ProgramHolder?.Programs?.FirstOrDefault(program => program.Name == programToFind);
                    if (program != null)
                    {
                        NetworkEntity?.EntityLog?.Info(CommandName, ": Program found. Saving program to send...");

                        var programStatus = ProgramStatus.FromProgram(program);
                        if (programStatus != null)
                        {
                            var rawJson = programStatus.ToJson();
                            if (rawJson != null)
                            {
                                NetworkEntity?.EntityLog?.Info(CommandName, ": Program saved.");

                                return message.CreateResponseMessage(rawJson);
                            }
                            else
                            {
                                return message.CreateFailureResponse("The program couldn't be sent as it couldn't be converted properly.");
                            }
                        }
                        else
                        {
                            return message.CreateFailureResponse("The program information couldn't be created.");
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
