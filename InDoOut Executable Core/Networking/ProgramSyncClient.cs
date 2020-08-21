using InDoOut_Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public class ProgramSyncClient : AbstractClient
    {
        private readonly ProgramSyncProgramCommandTracker _commandTracker = new ProgramSyncProgramCommandTracker();

        public ProgramSyncClient() : base()
        {
        }

        public async Task<List<string>> RequestAvailablePrograms() => await RequestAvailablePrograms(CancellationToken.None);
        public async Task<List<string>> RequestAvailablePrograms(CancellationToken cancellationToken)
        {
            var response = await SendCommandAndWait("REQUEST_PROGRAMS", cancellationToken);
            
            return response?.Split("\n", StringSplitOptions.RemoveEmptyEntries)?.ToList();
        }

        protected override void MessageReceived(string message)
        {
            var command = ProgramSyncCommand.ExtractFromCommandString(message);
            if (command?.Valid ?? false)
            {
                if (_commandTracker.CanBeProcessed(command))
                {
                    _ = Task.Run(() => _commandTracker.RespondToAnyQueuedMessages(command));
                }
                else
                {
                    Log.Instance.Warning("A command seems to have fallen through while attempting to process it as it wasn't valid.");
                } 
            }
            else
            {
                Log.Instance.Error("Couldn't format the command from a message received by the server inside program sync client! Length: ", message?.Length);
            }
        }

        private async Task<string> SendCommandAndWait(string name, CancellationToken cancellationToken, string data = null)
        {
            var createdCommand = _commandTracker.CreateAndQueueCommand(name, data, out var uniqueName);
            if ((createdCommand?.Valid ?? false) && await Send(createdCommand.FullCommandString))
            {
                var response = await _commandTracker.AwaitCommandResponse(uniqueName, cancellationToken);
                if (response?.Valid ?? false)
                {
                    return response.Data ?? "OK";
                }
            }
            else
            {
                _ = _commandTracker.RemoveFromResponseQueue(uniqueName);
            } 

            return null;
        }
    }
}
