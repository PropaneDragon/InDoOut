using InDoOut_Core.Entities.Programs;
using InDoOut_Executable_Core.Storage;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking.Commands
{
    public class UploadProgramClientCommand : Command<IClient>
    {
        public override string CommandName => "UPLOAD_PROGRAM";

        public UploadProgramClientCommand(IClient client) : base(client)
        {
        }

        public async Task<bool> SendProgram(IProgram program, CancellationToken cancellationToken)
        {
            if (program != null && program.Metadata.TryGetValue(ProgramStorer.PROGRAM_METADATA_LAST_LOADED_FROM, out var programLastLoadedFrom) && !string.IsNullOrEmpty(program.Name) && !string.IsNullOrEmpty(programLastLoadedFrom))
            {
                var programData = "";

                try
                {
                    if (File.Exists(programLastLoadedFrom))
                    {
                        programData = File.ReadAllText(programLastLoadedFrom);
                    }
                }
                catch
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(programData))
                {
                    return await SendProgram(program.Name, programData, cancellationToken);
                }
            }

            return false;
        }

        public async Task<bool> SendProgram(string programName, string programData, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(programName))
            {
                if (!string.IsNullOrWhiteSpace(programData))
                {
                    var response = await SendMessageAwaitResponse(cancellationToken, programName, programData);

                    return response != null && response.Valid && response.SuccessMessage != null;
                }
            }

            return false;
        }
    }
}
