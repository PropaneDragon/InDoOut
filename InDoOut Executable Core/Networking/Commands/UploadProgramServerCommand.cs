using InDoOut_Executable_Core.Location;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking.Commands
{
    public class UploadProgramServerCommand : CommandListener<IServer>
    {
        public override string CommandName => "UPLOAD_PROGRAM";

        public string StoredProgramsLocation { get; set; } = $"{StandardLocations.Instance.GetPathTo(Location.Location.ApplicationDirectory)}{Path.DirectorySeparatorChar}Programs{Path.DirectorySeparatorChar}Synced";


        public UploadProgramServerCommand(IServer server) : base(server)
        {
        }

        public override async Task<INetworkMessage> CommandReceived(INetworkMessage command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (command != null && command.Valid && (command.Data?.Length ?? 0) == 2)
            {
                var programName = command.Data[0];
                var programData = command.Data[1];

                if (!string.IsNullOrWhiteSpace(programName) && !string.IsNullOrWhiteSpace(programData) && !programName.Any(character => Path.GetInvalidFileNameChars().Contains(character)))
                {
                    try
                    {
                        if (!Directory.Exists(StoredProgramsLocation))
                        {
                            _ = Directory.CreateDirectory(StoredProgramsLocation);
                        }
                    }
                    catch (Exception ex)
                    {
                        return command.CreateFailureResponse($"Couldn't create a directory to store the given file. {ex.Message}");
                    }

                    try
                    {
                        File.WriteAllText($"{StoredProgramsLocation}{Path.DirectorySeparatorChar}{programName}.ido", programData);

                        return command.CreateSuccessResponse();
                    }
                    catch (Exception ex)
                    {
                        return command.CreateFailureResponse($"Couldn't create a file in the save directory. {ex.Message}");
                    }
                }
                else
                {
                    return command.CreateFailureResponse($"The sent data didn't have the correct information present to construct a program.");
                }
            }

            return command.CreateFailureResponse("The program received appeared to be invalid and can't be parsed.");
        }
    }
}
