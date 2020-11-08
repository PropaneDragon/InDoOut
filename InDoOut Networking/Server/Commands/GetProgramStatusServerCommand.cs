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
                var matchedProgram = ProgramHolder?.Programs?.FirstOrDefault(program => program.Id == programId);
                if (matchedProgram != null)
                {
                    var status = ProgramStatus.FromProgram(matchedProgram)?.ToJson();
                    if (status != null)
                    {
                        return message.CreateResponseMessage(status);
                    }
                }
            }

            return message.CreateFailureResponse("The program ID was an invalid format and couldn't be parsed.");
        }
    }
}
