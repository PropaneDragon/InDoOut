using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Entities
{
    public class NetworkedProgram : INetworkedProgram
    {
        private string _name = null;

        public bool Connected => AssociatedClient?.Connected ?? false;
        public bool Running { get; private set; } = false;
        public bool Stopping { get; private set; } = false;
        public bool Finishing { get; private set; } = false;

        public string Name { get => _name != null ? $"{_name} [{(Connected ? "Connected" : "Disconnected")}]" : null; set => _name = value; }
        public string ReturnCode { get; set; }

        public IClient AssociatedClient { get; protected set; } = null;

        public List<IFunction> Functions { get; } = new List<IFunction>();
        public List<IStartFunction> StartFunctions => new List<IStartFunction>();
        public List<IEndFunction> EndFunctions => new List<IEndFunction>();
        public List<string> PassthroughValues => new List<string>();

        public DateTime LastTriggerTime { get; set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; set; } = DateTime.MinValue;

        public Guid Id { get; set; } = Guid.NewGuid();

        public Dictionary<string, string> Metadata { get; } = new Dictionary<string, string>();

        private NetworkedProgram()
        {
        }

        public NetworkedProgram(IClient client) : this()
        {
            AssociatedClient = client;
        }

        public bool UpdateFromStatus(ProgramStatus status)
        {
            if (status != null)
            {
                var propertyExtractor = new PropertyExtractor<ProgramStatus, NetworkedProgram>(status);
                var convertedAll = UpdateFunctionsFromStatus(status);

                return propertyExtractor.ApplyTo(this) && convertedAll;
            }

            return false;
        }

        private bool UpdateFunctionsFromStatus(ProgramStatus status)
        {
            var convertedAll = true;

            foreach (var functionStatus in status.Functions)
            {
                var foundFunction = Functions.FirstOrDefault(function => function.Id == functionStatus.Id);
                if (foundFunction is INetworkedFunction networkedFunction)
                {
                    convertedAll = networkedFunction.UpdateFromStatus(functionStatus) && convertedAll;
                }
                else
                {
                    var functionToAdd = new NetworkedFunction();
                    convertedAll = functionToAdd.UpdateFromStatus(functionStatus) && convertedAll;

                    Functions.Add(functionToAdd);
                }
            }

            return convertedAll;
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
                        return UpdateFromStatus(status);
                    }
                }
                catch { }
            }

            return false;
        }

        public async Task<bool> Disconnect() => Connected && await AssociatedClient?.Disconnect();

        public void Stop() { } //Todo - Send data to network

        public void SetName(string name) => Name = name;

        public bool AddFunction(IFunction function) => false;

        public bool RemoveFunction(IFunction function) => false;

        public void Trigger(IEntity triggeredBy) { }

        public bool CanBeTriggered(IEntity entity) => false;

        public bool HasBeenTriggeredSince(DateTime time) => false; //Todo

        public bool HasBeenTriggeredWithin(TimeSpan time) => false; //Todo

        public bool HasCompletedSince(DateTime time) => false; //Todo

        public bool HasCompletedWithin(TimeSpan time) => false; //Todo
    }
}
