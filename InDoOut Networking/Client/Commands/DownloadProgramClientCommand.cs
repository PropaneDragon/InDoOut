using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Networking.Entities;
using InDoOut_Networking.Shared.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Client.Commands
{
    public class DownloadProgramClientCommand : Command<IClient>
    {
        public DownloadProgramClientCommand(IClient client) : base(client)
        {
        }

        public async Task<string> RequestDataForProgramAsync(string programName, CancellationToken cancellationToken)
        {
            var response = await SendMessageAwaitResponse(cancellationToken, programName);

            return (response?.Data?.Length ?? 0) == 1 ? response.Data[0] : null;
        }

        public async Task<bool> RequestProgramAsync(string programName, INetworkedProgram programToLoadInto, CancellationToken cancellationToken)
        {
            var data = await RequestDataForProgramAsync(programName, cancellationToken);
            if (programToLoadInto != null && !string.IsNullOrEmpty(data))
            {
                var programStatus = ProgramStatus.FromJson(data);
                if (programStatus != null)
                {
                    return programToLoadInto.UpdateFromStatus(programStatus, true);
                }
            }

            return false;
        }
    }
}
