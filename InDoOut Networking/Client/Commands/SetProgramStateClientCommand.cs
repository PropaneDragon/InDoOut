using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Networking.Shared.Commands;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Client.Commands
{
    public class SetProgramStateClientCommand : Command<IClient>
    {
        public SetProgramStateClientCommand(IClient client) : base(client)
        {
        }

        public async Task<bool> SetProgramState(Guid id, SetProgramStateShared.ProgramState state, CancellationToken cancellationToken)
        {
            var response = await SendMessageAwaitResponse(cancellationToken, id.ToString(), ((int)state).ToString());
            var success = response?.IsSuccessMessage ?? false;

            return success;
        }
    }
}
