using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Server.Commands
{
    public class GetProgramStatusServerCommand : CommandListener<IServer>
    {
        public IProgramHolder ProgramHolder { get; set; } = null;

        public GetProgramStatusServerCommand(IServer server, IProgramHolder programHolder) : base(server)
        {
            ProgramHolder = programHolder;
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if ((message?.Data?.Length ?? 0) == 1 && Guid.TryParse(message.Data[0], out var programId))
            {
                NetworkEntity?.EntityLog?.Info(CommandName, ": Finding program with ID - \"", programId, "\"...");

                var matchedProgram = ProgramHolder?.Programs?.FirstOrDefault(program => program.Id == programId);
                if (matchedProgram != null)
                {
                    NetworkEntity?.EntityLog?.Info(CommandName, ": Program found. Generating status...");

                    var status = ProgramStatus.FromProgram(matchedProgram)?.ToJson();
                    if (status != null)
                    {
                        NetworkEntity?.EntityLog?.Info(CommandName, ": Sending status.");

                        return message.CreateResponseMessage(status);
                    }
                    else
                    {
                        return message.CreateFailureResponse("Couldn't get program status.");
                    }
                }
                else
                {
                    return message.CreateFailureResponse("The program couldn't be found.");
                }
            }

            return message.CreateFailureResponse($"The request appears to be invalid and can't be accepted by the server.");
        }
    }
}
