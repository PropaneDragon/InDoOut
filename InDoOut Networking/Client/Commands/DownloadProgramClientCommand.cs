using InDoOut_Executable_Core.Networking.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Client.Commands
{
    public class DownloadProgramClientCommand : Command<IClient>
    {
        public DownloadProgramClientCommand(IClient client) : base(client)
        {
        }

        public override string CommandName => "DOWNLOAD_PROGRAM";

        public async Task<string> RequestDataForProgram(string programName, CancellationToken cancellationToken)
        {
            var response = await SendMessageAwaitResponse(cancellationToken, programName);

            return (response?.Data?.Length ?? 0) == 1 ? response.Data[0] : null;
        }
    }
}
