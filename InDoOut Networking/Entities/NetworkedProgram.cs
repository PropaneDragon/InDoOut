using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Logging;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Entities
{
    public class NetworkedProgram : INetworkedProgram
    {
        private IProgram _internalProgram = new Program();

        public bool Connected => AssociatedClient?.Connected ?? false;
        public bool Running { get; private set; }
        public bool Stopping => false; //Todo - Synchronise with networked data.
        public bool Finishing => false; //Todo - Synchronise with networked data.

        public string Name => AssociatedProgram?.Name != null ? $"{AssociatedProgram?.Name} [{(Connected ? "Connected" : "Disconnected")}]" : null;
        public string ReturnCode => AssociatedProgram?.ReturnCode;

        public IClient AssociatedClient { get; protected set; }
        public IProgram AssociatedProgram { get => _internalProgram; set => PopulateFromProgram(value); }

        public List<IFunction> Functions => AssociatedProgram?.Functions;
        public List<IStartFunction> StartFunctions => AssociatedProgram?.StartFunctions;
        public List<IEndFunction> EndFunctions => AssociatedProgram?.EndFunctions;
        public List<string> PassthroughValues => AssociatedProgram?.PassthroughValues;

        public DateTime LastTriggerTime => DateTime.Now; //Todo - Synchronise with networked data.
        public DateTime LastCompletionTime => DateTime.Now; //Todo - Synchronise with networked data.

        public Guid Id { get => AssociatedProgram?.Id ?? Guid.Empty; set => AssociatedProgram.Id = value; }

        public Dictionary<string, string> Metadata => AssociatedProgram?.Metadata;

        private NetworkedProgram(params string[] _)
        {
        }

        public NetworkedProgram(IClient client) : this()
        {
            AssociatedClient = client;
        }

        public NetworkedProgram(IClient client, IProgram program) : this(client)
        {
            AssociatedProgram = program;
        }

        public async Task<bool> Reload(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (Connected && !string.IsNullOrEmpty(Name))
            {
                try
                {

                }
                catch { }
            }

            return false;
        }

        public async Task<bool> Synchronise(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (Connected && !string.IsNullOrEmpty(Name))
            {
                try
                {
                    var programStatusCommand = new GetProgramStatusClientCommand(AssociatedClient);
                    var status = await programStatusCommand.GetProgramStatusAsync(Id, cancellationToken);

                    if (status != null)
                    {
                        return UpdateFromProgramStatus(status);
                    }
                }
                catch { }
            }

            return false;
        }

        public async Task<bool> Disconnect() => Connected && await AssociatedClient?.Disconnect();

        public void Stop() { } //Todo - Send data to network

        public void SetName(string name) => AssociatedProgram?.SetName(name);

        public bool AddFunction(IFunction function) => false;

        public bool RemoveFunction(IFunction function) => false;

        public void Trigger(IEntity triggeredBy) { }

        public bool CanBeTriggered(IEntity entity) => false;

        public bool HasBeenTriggeredSince(DateTime time) => false; //Todo

        public bool HasBeenTriggeredWithin(TimeSpan time) => false; //Todo

        public bool HasCompletedSince(DateTime time) => false; //Todo

        public bool HasCompletedWithin(TimeSpan time) => false; //Todo

        private void PopulateFromProgram(IProgram program)
        {
            _internalProgram = program;

            if (program != null && !(program is INetworkedProgram))
            {
                _ = UpdateFunctions(program);
            }
            else
            {
                Log.Instance.Error("Attempted to populate a networked program from ", program, ", which is invalid.");
            }
        }

        private bool UpdateFromProgramStatus(ProgramStatus status)
        {
            if (status != null && status.Id == Id)
            {
                foreach (var function in Functions)
                {
                    if (function is INetworkedFunction networkedFunction)
                    {
                        if (networkedFunction.UpdateFromStatus(status))
                        {
                            return true;
                        }
                        else
                        {
                            Log.Instance.Error("Failed to update function ", networkedFunction?.Id, " from given ProgramStatus. This possibly means we're out of sync.");
                        }
                    }
                    else
                    {
                        Log.Instance.Error("The function ", function?.Id, " is not a NetworkedFunction and cannot be updated. This state shouldn't have happened and needs investigation.");
                    }
                }
            }
            else
            {
                Log.Instance.Error("The given status ", status?.Id, " doesn't match the ID of the program it has been run on (", Id, ") and can't be updated.");
            }

            return false;
        }

        private bool UpdateFunctions(IProgram program)
        {
            var applied = true;

            Functions.Clear();

            foreach (var function in program.Functions)
            {
                if (!(function is INetworkedFunction))
                {
                    var networkedFunction = new NetworkedFunction(function);

                    Functions.Add(networkedFunction);
                }
                else
                {
                    Log.Instance.Error("Attempted to update a function that was already a networked function. This indicates something has gone wrong internally.");

                    applied = false;
                }
            }

            return applied;
        }
    }
}
