using InDoOut_Executable_Core.Networking.Commands;
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

        public override string CommandName => "REQUEST_PROGRAMS";

        public async Task<List<string>> RequestAvailablePrograms(CancellationToken cancellationToken)
        {
            var response = await SendMessageAwaitResponse(cancellationToken);

            return response != null ? response.Data?.Where(item => !string.IsNullOrWhiteSpace(item))?.ToList() ?? new List<string>() : null;
        }
    }
}
