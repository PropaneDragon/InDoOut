using InDoOut_Core.Entities.Programs;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Client.Commands
{
    public class GetProgramStatusClientCommand : Command<IClient>
    {
        public GetProgramStatusClientCommand(IClient client) : base(client)
        {
        }

        public async Task<ProgramStatus> GetProgramStatusAsync(IProgram program, CancellationToken cancellationToken) => program != null ? await GetProgramStatusAsync(program.Id, cancellationToken) : null;

        public async Task<ProgramStatus> GetProgramStatusAsync(Guid programGuid, CancellationToken cancellationToken)
        {
            var responseData = await SendMessageAwaitResponse(cancellationToken, programGuid.ToString());
            
            return (responseData?.Data?.Length ?? 0) == 1 ? ProgramStatus.FromJson(responseData.Data[0]) : null;
        }
    }
}
