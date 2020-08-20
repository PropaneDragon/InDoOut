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
        protected static readonly string SPLIT_IDENTIFIER_STRING = "\u0004\u0002\u0004";

        private readonly object _responseQueueLock = new object();
        private readonly Dictionary<string, ProgramSyncCommand> _responseQueue = new Dictionary<string, ProgramSyncCommand>();

        public ProgramSyncClient() : base()
        {
        }

        public async Task<List<string>> RequestAvailablePrograms() => await RequestAvailablePrograms(CancellationToken.None);
        public async Task<List<string>> RequestAvailablePrograms(CancellationToken cancellationToken)
        {
            var response = await SendCommandAndWait("REQUEST_PROGRAMS", cancellationToken);
            
            return !string.IsNullOrWhiteSpace(response) ? response.Split("\n").ToList() : null;
        }

        protected override void MessageReceived(string message)
        {
            var command = ProgramSyncCommand.ExtractFromCommandString(message);
            if (command?.Valid ?? false)
            {
                _ = Task.Run(() => ProcessCommand(command));
            }
            else
            {
                Log.Instance.Error("Couldn't format the command from a message received by the server inside program sync client! Length: ", message?.Length);
            }
        }

        protected void ProcessCommand(ProgramSyncCommand command)
        {
            var splitCommand = command.Command.Split(SPLIT_IDENTIFIER_STRING);
            if (splitCommand.Length > 1)
            {
                var id = splitCommand[0];
                var everythingElse = string.Join("", splitCommand[1..]);

                lock (_responseQueueLock)
                {
                    if (_responseQueue.ContainsKey(id))
                    {
                        _responseQueue[id] = command;
                    }
                    else
                    {
                        Log.Instance.Error("Couldn't find a response to a command sent by the server. Length: ", everythingElse.Length);
                    } 
                }
            }
            else
            {
                Log.Instance.Error("Command had no identifier to trace back to the request!");

                //Todo: Any other commands that might need processing that won't have an identifier?
            } 
        }

        private async Task<string> SendCommandAndWait(string name, CancellationToken cancellationToken, string data = null)
        {
            var id = Guid.NewGuid();
            var fullCommand = new ProgramSyncCommand($"{id}{SPLIT_IDENTIFIER_STRING}{name}", data);

            if (fullCommand.Valid)
            {
                AddToResponseQueue(id.ToString());
                
                if (await Send(fullCommand.FullCommandString))
                {
                    var response = await AwaitCommandResponse(id.ToString(), cancellationToken);
                    if (response?.Valid ?? false)
                    {
                        return response.Data ?? "OK";
                    }
                }
                else
                {
                    _ = RemoveFromResponseQueue(id.ToString());
                } 
            }

            return null;
        }

        private async Task<ProgramSyncCommand> AwaitCommandResponse(string commandName, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    lock (_responseQueueLock)
                    {
                        if (_responseQueue.ContainsKey(commandName))
                        {
                            if (_responseQueue[commandName] != null)
                            {
                                var command = _responseQueue[commandName];

                                _ = RemoveFromResponseQueue(commandName);

                                return command;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(50));
                }

                return null;
            });
        }

        private void AddToResponseQueue(string commandName)
        {
            lock (_responseQueueLock)
            {
                _responseQueue[commandName] = null;
            }
        }

        private bool RemoveFromResponseQueue(string commandName)
        {
            lock (_responseQueueLock)
            {
                if (_responseQueue.ContainsKey(commandName))
                {
                    _ = _responseQueue.Remove(commandName);

                    return true;
                }
            }

            return false;
        }
    }
}
