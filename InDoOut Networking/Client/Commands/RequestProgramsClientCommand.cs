using InDoOut_Executable_Core.Networking.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Client.Commands
{
    public class RequestProgramsClientCommand : Command<IClient>
    {
        public RequestProgramsClientCommand(IClient client) : base(client)
        {
        }

        public async Task<List<Guid>> RequestAvailableProgramsAsync(CancellationToken cancellationToken)
        {
            var response = await SendMessageAwaitResponse(cancellationToken);

            return response != null ? response.Data?.Where(id => !string.IsNullOrWhiteSpace(id))?.Select(idString => Guid.TryParse(idString, out var id) ? id : Guid.Empty).ToList() ?? new List<Guid>() : null;
        }
    }
}
