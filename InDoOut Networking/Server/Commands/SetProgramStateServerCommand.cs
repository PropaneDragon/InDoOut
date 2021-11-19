using InDoOut_Core.Entities.Programs;
using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Shared.Commands;
using InDoOut_Networking.Shared.Commands.Exceptions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Server.Commands
{
    public class SetProgramStateServerCommand : CommandListener<IServer>
    {
        public IProgramHolder ProgramHolder { get; private set; } = null;

        public SetProgramStateServerCommand(IServer server, IProgramHolder programHolder) : base(server)
        {
            ProgramHolder = programHolder;
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage message, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (ProgramHolder != null && (message?.Data?.Length ?? -1) == 2)
            {
                NetworkEntity?.EntityLog?.Info(CommandName, ": Setting program state...");

                var data = message.Data;

                if (Guid.TryParse(data[0], out var programId))
                {
                    if (Enum.TryParse<SetProgramStateShared.ProgramState>(data[1], out var state))
                    {
                        var foundProgram = ProgramHolder?.Programs.FirstOrDefault(program => program.Id == programId);
                        if (foundProgram != null)
                        {
                            NetworkEntity?.EntityLog?.Info(CommandName, ": Found program: ", foundProgram.Name, ".");

                            if (ApplyStateToProgram(foundProgram, state))
                            {
                                return message.CreateSuccessResponse();
                            }
                        }
                        else
                        {
                            return message.CreateFailureResponse($"The program couldn't be found on the server.");
                        }
                    }
                    else
                    {
                        return message.CreateFailureResponse($"The program state appears to be invalid and couldn't be processed.");
                    }
                }
                else
                {
                    return message.CreateFailureResponse($"The program id appears to be invalid and couldn't be processed.");
                }
            }

            return message.CreateFailureResponse($"The request appears to be invalid and can't be accepted by the server.");
        }

        private bool ApplyStateToProgram(IProgram program, SetProgramStateShared.ProgramState state)
        {
            if (program != null)
            {
                switch (state)
                {
                    case SetProgramStateShared.ProgramState.Start:
                        program.Trigger(null);
                        break;
                    case SetProgramStateShared.ProgramState.Stop:
                        program.Stop();
                        break;
                    default:
                        throw new CommandFailureException($"An invalid state was given to the program ({state}) and it can't be handled.");
                }

                return true;
            }

            return false;
        }
    }
}
