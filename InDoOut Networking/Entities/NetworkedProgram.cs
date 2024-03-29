﻿using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Extensions.Time;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Shared.Commands;
using InDoOut_Networking.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace InDoOut_Networking.Entities
{
    public class NetworkedProgram : INetworkedProgram
    {
        private readonly object _functionLock = new();
        private readonly object _metadataLock = new();
        private readonly object _updateLock = new();

        private readonly List<IFunction> _functions = new();
        private readonly Dictionary<string, string> _metadata = new();
        private readonly System.Timers.Timer _timer = new() { AutoReset = false, Interval = TimeSpan.FromSeconds(1).TotalMilliseconds };

        private string _name = null;

        public bool Connected => AssociatedClient?.Connected ?? false;
        public bool Running { get; private set; } = false;
        public bool Stopping { get; private set; } = false;
        public bool Finishing { get; private set; } = false;
        public bool Updating { get; private set; } = false;

        public string Name { get => $"{_name ?? "Untitled"}"; set => _name = value; }
        public string ReturnCode { get; set; }

        public IClient AssociatedClient { get; protected set; } = null;

        public List<IFunction> Functions { get { lock (_functionLock) { return _functions; } } }
        public List<IStartFunction> StartFunctions => new();
        public List<IEndFunction> EndFunctions => new();
        public List<string> PassthroughValues => new();

        public DateTime LastUpdateTime { get; set; } = DateTime.MinValue;
        public DateTime LastTriggerTime { get; set; } = DateTime.MinValue;
        public DateTime LastCompletionTime { get; set; } = DateTime.MinValue;
        public TimeSpan FastUpdateInterval { get; set; } = TimeSpan.FromMilliseconds(200);
        public TimeSpan SlowUpdateInterval { get; set; } = TimeSpan.FromSeconds(1);

        public Guid Id { get; set; } = Guid.NewGuid();

        public Dictionary<string, string> Metadata { get { lock (_metadataLock) { return _metadata; } } }

        private NetworkedProgram()
        {
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        public NetworkedProgram(IClient client) : this()
        {
            AssociatedClient = client;
        }

        public bool UpdateFromStatus(ProgramStatus status, bool clearAllFirst = false)
        {
            var convertedAll = true;

            lock (_updateLock)
            {
                Updating = true;

                if (clearAllFirst)
                {
                    _ = Clear();
                }

                if (status != null)
                {
                    var propertyExtractor = new PropertyExtractor<ProgramStatus, NetworkedProgram>(status);
                    convertedAll = UpdateFunctionsFromStatus(status) && convertedAll;
                    convertedAll = UpdateConnectionsFromStatus(status) && convertedAll;
                    convertedAll = UpdateMetadataFromStatus(status) && convertedAll;

                    return propertyExtractor.ApplyTo(this) && convertedAll;
                }
                else
                {
                    convertedAll = false;
                }

                Updating = false;
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
                    var downloadProgramCommand = new DownloadProgramClientCommand(AssociatedClient);
                    return await downloadProgramCommand.RequestProgramAsync(Name, this, cancellationToken);
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
                        return UpdateFromStatus(status, false);
                    }
                }
                catch { }
            }

            return false;
        }

        public void Stop()
        {
            if (Connected)
            {
                var command = new SetProgramStateClientCommand(AssociatedClient);
                var token = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                _ = Task.Run(async () => await command.SetProgramState(Id, SetProgramStateShared.ProgramState.Stop, token.Token));
            }
        }

        public void Trigger(IEntity triggeredBy)
        {
            if (Connected)
            {
                var command = new SetProgramStateClientCommand(AssociatedClient);
                var token = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                _ = Task.Run(async () => await command.SetProgramState(Id, SetProgramStateShared.ProgramState.Start, token.Token));
            }
        }

        public async Task<bool> Disconnect() => Connected && await AssociatedClient?.Disconnect();
        public void SetName(string name) => Name = name;
        public bool AddFunction(IFunction function) => false;
        public bool RemoveFunction(IFunction function) => false;
        public bool CanBeTriggered(IEntity entity) => false;
        public bool HasBeenTriggeredSince(DateTime time) => LastTriggerTime.HasOccurredSince(time);
        public bool HasBeenTriggeredWithin(TimeSpan time) => LastTriggerTime.HasOccurredWithin(time, LastUpdateTime);
        public bool HasCompletedSince(DateTime time) => LastCompletionTime.HasOccurredSince(time);
        public bool HasCompletedWithin(TimeSpan time) => LastCompletionTime.HasOccurredWithin(time, LastUpdateTime);

        private bool Clear()
        {
            lock (_functionLock)
            {
                Functions.Clear();
                StartFunctions.Clear();
                EndFunctions.Clear();
            }

            lock (_metadataLock)
            {
                Metadata.Clear();
            }

            PassthroughValues.Clear();

            return true;
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

        private bool UpdateConnectionsFromStatus(ProgramStatus status)
        {
            var convertedAll = true;

            foreach (var connectionStatus in status.Connections)
            {
                var start = Functions.FirstOrDefault(function => function.Id == connectionStatus.StartFunctionId);
                var end = Functions.FirstOrDefault(function => function.Id == connectionStatus.EndFunctionId);
                var outputName = connectionStatus.OutputName;
                var inputName = connectionStatus.InputName;
                var connectionType = connectionStatus.TypeOfConnection;

#pragma warning disable IDE0045 // Convert to conditional expression
                if (start != null && end != null && !string.IsNullOrEmpty(outputName) && !string.IsNullOrEmpty(inputName) && connectionType != ConnectionStatus.ConnectionType.Unknown)
                {
                    convertedAll = connectionType switch
                    {
                        InDoOut_Json_Storage.JsonConnection.ConnectionType.InputOutput => LinkInputOutput(connectionStatus, start, end, outputName, inputName),
                        InDoOut_Json_Storage.JsonConnection.ConnectionType.PropertyResult => LinkPropertyResult(connectionStatus, start, end, outputName, inputName),
                        _ => false,
                    };
                }
                else
                {
                    convertedAll = false;
                }
            }
#pragma warning restore IDE0045 // Convert to conditional expression

            return convertedAll;
        }

        private bool UpdateMetadataFromStatus(ProgramStatus status)
        {
            if (status != null)
            {
                Metadata.Clear();

                foreach (var pair in status.Metadata)
                {
                    Metadata.Add(pair.Key, pair.Value);
                }

                return true;
            }

            return false;
        }

        private bool LinkInputOutput(ConnectionStatus connection, IFunction startFunction, IFunction endFunction, string outputName, string inputName)
        {
            var convertedAll = true;

            if (startFunction != null && endFunction != null)
            {
                var output = startFunction.Outputs.FirstOrDefault(output => output.Name == outputName);
                var input = endFunction.Inputs.FirstOrDefault(input => input.Name == inputName);

                if (output != null && input != null && output.Connect(input))
                {
                    SyncMetadata(connection, input, output);
                }
            }

            return convertedAll;
        }

        private bool LinkPropertyResult(ConnectionStatus connection, IFunction startFunction, IFunction endFunction, string outputName, string inputName)
        {
            var convertedAll = true;

            if (startFunction != null && endFunction != null)
            {
                var result = startFunction.Results.FirstOrDefault(output => output.Name == outputName);
                var property = endFunction.Properties.FirstOrDefault(input => input.Name == inputName);

                if (result != null && property != null && result.Connect(property))
                {
                    SyncMetadata(connection, property, result);
                }
            }

            return convertedAll;
        }

        private void SyncMetadata(ConnectionStatus connection, IInputable inputable, IOutputable outputable)
        {
            if (outputable is IStored storedOutputable)
            {
                foreach (var metadata in connection.OutputMetadata)
                {
                    storedOutputable.Metadata[metadata.Key] = metadata.Value;
                }
            }

            if (inputable is IStored storedInputable)
            {
                foreach (var metadata in connection.InputMetadata)
                {
                    storedInputable.Metadata[metadata.Key] = metadata.Value;
                }
            }
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();

            using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            _ = await Synchronise(cancellationTokenSource.Token);

            _timer.Interval = (Running ? FastUpdateInterval : SlowUpdateInterval).TotalMilliseconds;
            _timer.Start();
        }
    }
}