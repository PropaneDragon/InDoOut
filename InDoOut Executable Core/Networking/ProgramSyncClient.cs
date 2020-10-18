using InDoOut_Core.Logging;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public class ProgramSyncClient : Client, IProgramSyncClient
    {
        public ProgramSyncClient() : base()
        {
        }

        protected override void MessageReceived(string message)
        {
            var command = ClientServerCommand.FromCommandString(message);
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
    }
}
