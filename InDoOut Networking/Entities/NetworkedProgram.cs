using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Logging;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Shared.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking.Entities
{
    public class NetworkedProgram : Program, INetworkedProgram
    {
        public override string Name { get => base.Name != null ? $"{base.Name} [{(Connected ? "Connected" : "Disconnected")}]" : null; protected set => base.Name = value; }

        public bool Connected => AssociatedClient?.Connected ?? false;

        public IClient AssociatedClient { get; protected set; }

        private NetworkedProgram(params string[] passthroughValues) : base(passthroughValues)
        {
        }

        public NetworkedProgram(IClient client) : this()
        {
            AssociatedClient = client;
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

        public override bool AddFunction(IFunction function) => false;
        public override bool CanBeTriggered(IEntity entity) => false;
        public override bool RemoveFunction(IFunction function) => false;
        public override void Stop() { }
        public override void Trigger(IEntity triggeredBy) { }

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
    }
}
