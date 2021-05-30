using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Executable_Core.Programs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Server.Commands
{
    public class RequestProgramsServerCommand : CommandListener<IServer>
    {
        public IProgramHolder ProgramHolder { get; set; } = null;

        public RequestProgramsServerCommand(IServer server, IProgramHolder programHolder) : base(server)
        {
            ProgramHolder = programHolder;
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (ProgramHolder != null)
            {
                NetworkEntity?.EntityLog?.Info(CommandName, ": Finding all stored programs...");

                var programs = ProgramHolder?.Programs?.Select(program => program?.Id.ToString() ?? "").ToArray();
                if (programs != null)
                {
                    NetworkEntity?.EntityLog?.Info(CommandName, ": Found ", programs?.Count(), " programs.");

                    return message.CreateResponseMessage(programs);
                }
                else
                {
                    NetworkEntity?.EntityLog?.Error(CommandName, ": There was a problem finding programs on the server.");
                    return message.CreateFailureResponse($"There was a problem finding programs on the server.");
                }
            }

            NetworkEntity?.EntityLog?.Error(CommandName, ": The request appears to be invalid and can't be accepted by the server.");
            return message.CreateFailureResponse($"The request appears to be invalid and can't be accepted by the server.");
        }
    }
}
