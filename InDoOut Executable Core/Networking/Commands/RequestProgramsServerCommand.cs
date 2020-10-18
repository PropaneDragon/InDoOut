﻿using InDoOut_Executable_Core.Programs;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking.Commands
{
    public class RequestProgramsServerCommand : CommandListener<IServer>
    {
        public override string CommandName => "REQUEST_PROGRAMS";

        public IProgramHolder ProgramHolder { get; set; } = null;

        public RequestProgramsServerCommand(IServer server, IProgramHolder programHolder) : base(server)
        {
            ProgramHolder = programHolder;
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (ProgramHolder != null)
            {
                var programs = ProgramHolder?.Programs?.Select(program => program?.Name ?? "").ToArray();
                if (programs != null)
                {
                    return command.CreateResponseMessage(programs);
                }
            }

            return null;
        }
    }
}
