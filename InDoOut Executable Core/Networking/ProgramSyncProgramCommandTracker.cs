using InDoOut_Core.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public class ProgramSyncProgramCommandTracker
    {
        public static readonly string SPLIT_IDENTIFIER_STRING = "\u0004\u0002\u0004";

        private readonly object _responseQueueLock = new object();
        private readonly Dictionary<string, ProgramSyncCommand> _responseQueue = new Dictionary<string, ProgramSyncCommand>();

        public bool CanBeProcessed(ProgramSyncCommand command) => command.Valid && command.Command.Contains(SPLIT_IDENTIFIER_STRING);

        public string GetIdentifierFromCommand(ProgramSyncCommand command)
        {
            var splitCommand = command.Command.Split(SPLIT_IDENTIFIER_STRING);

            return splitCommand.Length > 1 ? splitCommand[0] : null;
        }

        public string GetCommandWithoutIdentifier(ProgramSyncCommand command)
        {
            var splitCommand = command.Command.Split(SPLIT_IDENTIFIER_STRING);

            return splitCommand.Length > 1 ? string.Join("", splitCommand[1..]) : null;
        }

        public bool RespondToAnyQueuedMessages(ProgramSyncCommand command)
        {
            if (CanBeProcessed(command))
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

                            return true;
                        }
                        else
                        {
                            Log.Instance.Error("Couldn't process a command as it's not available in the queue. Length: ", everythingElse.Length);
                        }
                    }
                }
                else
                {
                    Log.Instance.Error("Assumed the command could be processed, however it appears it can't be.");
                }
            }
            else
            {
                Log.Instance.Error("Command had no identifier to trace back to the request!");

                //Todo: Any other commands that might need processing that won't have an identifier?
            }

            return false;
        }

        public ProgramSyncCommand CreateAndQueueCommand(string name, out string uniqueName) => CreateAndQueueCommand(name, null, out uniqueName);
        public ProgramSyncCommand CreateAndQueueCommand(string name, string data, out string uniqueName)
        {
            uniqueName = Guid.NewGuid().ToString();

            var fullCommand = new ProgramSyncCommand($"{uniqueName}{SPLIT_IDENTIFIER_STRING}{name}", data);

            if (fullCommand.Valid)
            {
                AddToResponseQueue(uniqueName);

                return fullCommand;
            }

            return null;
        }

        public bool RemoveFromResponseQueue(string uniqueName)
        {
            lock (_responseQueueLock)
            {
                if (uniqueName != null && _responseQueue.ContainsKey(uniqueName))
                {
                    _ = _responseQueue.Remove(uniqueName);

                    return true;
                }
            }

            return false;
        }

        public async Task<ProgramSyncCommand> AwaitCommandResponse(string uniqueName, CancellationToken cancellationToken)
        {
            return await Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    lock (_responseQueueLock)
                    {
                        if (_responseQueue.ContainsKey(uniqueName))
                        {
                            if (_responseQueue[uniqueName] != null)
                            {
                                var command = _responseQueue[uniqueName];

                                _ = RemoveFromResponseQueue(uniqueName);

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

        private void AddToResponseQueue(string uniqueName)
        {
            lock (_responseQueueLock)
            {
                _responseQueue[uniqueName] = null;
            }
        }
    }
}
