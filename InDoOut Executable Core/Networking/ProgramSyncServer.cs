using InDoOut_Core.Logging;
using InDoOut_Executable_Core.Networking.ServerEventArgs;
using InDoOut_Executable_Core.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public class ProgramSyncServer : AbstractServer
    {
        private readonly object _cachedCommandMethodsLock = new object();

        private readonly ProgramSyncProgramCommandTracker _commandTracker = new ProgramSyncProgramCommandTracker();
        private readonly Dictionary<string, Func<ProgramSyncCommand, ProgramSyncCommand>> _cachedCommandMethods = new Dictionary<string, Func<ProgramSyncCommand, ProgramSyncCommand>>();

        public IProgramHolder AssociatedProgramHolder { get; set; } = null;

        public ProgramSyncServer(IProgramHolder programHolder, int port = 0) : base(port)
        {
            AssociatedProgramHolder = programHolder;

            CacheCommandMethods();
        }

        protected override void ClientMessageReceived(TcpClient client, string message)
        {
            var command = ProgramSyncCommand.ExtractFromCommandString(message);
            if (command?.Valid ?? false)
            {
                if (_commandTracker.CanBeProcessed(command))
                {
                    _ = Task.Run(() =>
                    {
                        if (!_commandTracker.RespondToAnyQueuedMessages(command))
                        {
                            ProcessCommand(client, command);
                        }
                    });
                }
                else
                {
                    Log.Instance.Warning("A command seems to have fallen through while attempting to process it as it wasn't valid.");
                }
            }
            else
            {
                Log.Instance.Error("Couldn't format the command from a message received from the client inside program sync server! Length: ", message?.Length);
            }
        }

        [CommandResponse("REQUEST_PROGRAMS")]
        protected ProgramSyncCommand GetAvailablePrograms(ProgramSyncCommand originalCommand)
        {
            _ = originalCommand.Data;

            if (AssociatedProgramHolder != null)
            {
                var programs = string.Join('\n', AssociatedProgramHolder.Programs?.Select(program => program?.Name ?? ""));
                if (programs != null)
                {
                    return new ProgramSyncCommand(originalCommand.Command, programs);
                }
            }

            return null;
        }

        private void ProcessCommand(TcpClient client, ProgramSyncCommand command)
        {
            if (client != null && command.Valid)
            {
                var methodForCommand = GetMethodForCommand(command);
                if (methodForCommand != null)
                {
                    try
                    {
                        var response = methodForCommand.Invoke(command);
                        if (response?.Valid ?? false)
                        {
                            _ = Task.Run(async () => _ = await SendMessage(client, response.FullCommandString));
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error("Couldn't respond to message due to an exception: ", ex?.Message);
                    }
                }
                else
                {
                    var rawCommandName = _commandTracker.GetCommandWithoutIdentifier(command);

                    Log.Instance.Warning("A command was passed from the client that was not recognised and couldn't be processed. Length: ", rawCommandName?.Length);
                }
            }
        }

        private void CacheCommandMethods()
        {
            var methods = GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var method in methods)
            {
                var commandResponseAttribute = method.GetCustomAttribute<CommandResponseAttribute>();
                if (!string.IsNullOrWhiteSpace(commandResponseAttribute?.AssociatedCommandName) && method.ReturnType == typeof(ProgramSyncCommand))
                {
                    var methodParameters = method.GetParameters();
                    if (methodParameters.Length == 0 || (methodParameters.Length == 1 && methodParameters[0].ParameterType == typeof(ProgramSyncCommand)))
                    {
                        Log.Instance.Info("Cached method for command length: ", commandResponseAttribute?.AssociatedCommandName);

                        _cachedCommandMethods.Add(commandResponseAttribute.AssociatedCommandName, (command) => method.Invoke(this, new[] { command }) as ProgramSyncCommand);
                    }
                    else
                    {
                        Log.Instance.Error("Attempted to cache a method with an attribute, but it didn't have the correct parameters!");
                    }
                }
                else
                {
                    Log.Instance.Error("Attempted to cache a method with an attribute, but it was invalid!");
                }
            }
        }

        private Func<ProgramSyncCommand, ProgramSyncCommand> GetMethodForCommand(ProgramSyncCommand command)
        {
            var rawCommandName = _commandTracker.GetCommandWithoutIdentifier(command);
            if (!string.IsNullOrWhiteSpace(rawCommandName))
            {
                lock (_cachedCommandMethodsLock)
                {
                    if (_cachedCommandMethods.TryGetValue(rawCommandName, out var method) && method != null)
                    {
                        return method;
                    }
                    else
                    {
                        Log.Instance.Warning("No method found for command length: ", rawCommandName?.Length);
                    }
                }
            }
            else
            {
                Log.Instance.Error("Attempted to get a method for an empty command!");
            }

            return null;
        }

        private async Task<string> SendCommandAndWait(TcpClient client, string name, CancellationToken cancellationToken, string data = null)
        {
            var createdCommand = _commandTracker.CreateAndQueueCommand(name, data, out var uniqueName);
            if ((createdCommand?.Valid ?? false) && await SendMessage(client, createdCommand.FullCommandString))
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

        protected override void ClientConnected(TcpClient client) { }
        protected override void ClientDisconnected(TcpClient client) { }

        protected override bool CanAcceptClient(TcpClient client) => true; //Todo: Identify the client
    }
}
