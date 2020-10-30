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
        public override string CommandName => "REQUEST_PROGRAMS";

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
                var programs = ProgramHolder?.Programs?.Select(program => program?.Name ?? "").ToArray();
                if (programs != null)
                {
                    return message.CreateResponseMessage(programs);
                }
            }

            return null;
        }
    }
}
