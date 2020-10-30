using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Networking.Commands;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Client.Commands
{
    public class UploadProgramClientCommand : Command<IClient>
    {
        public override string CommandName => "UPLOAD_PROGRAM";

        public UploadProgramClientCommand(IClient client) : base(client)
        {
        }

        public async Task<bool> SendProgram(IProgram program, CancellationToken cancellationToken)
        {
            if (program != null && !string.IsNullOrEmpty(program.Name))
            {
                var programData = "";

                try
                {
                    using var memoryStream = new MemoryStream();

                    var storer = new ProgramJsonStorer(new FunctionBuilder(), LoadedPlugins.Instance, memoryStream);
                    var failures = storer.Save(program);

                    if (failures.Count == 0 && memoryStream.CanRead)
                    {
                        if (memoryStream.CanSeek)
                        {
                            _ = memoryStream.Seek(0, SeekOrigin.Begin);
                        }

                        using var reader = new StreamReader(memoryStream, leaveOpen: true);

                        programData = reader.ReadToEnd();
                    }
                }
                catch
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(programData))
                {
                    return await SendProgram(programData, cancellationToken);
                }
            }

            return false;
        }

        public async Task<bool> SendProgram(string programData, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(programData))
            {
                var response = await SendMessageAwaitResponse(cancellationToken, programData);

                return response != null && response.Valid && response.SuccessMessage != null;
            }

            return false;
        }
    }
}
