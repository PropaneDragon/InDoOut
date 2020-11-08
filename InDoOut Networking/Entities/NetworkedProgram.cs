﻿using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Networking.Client;
using System;
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

        public async Task<bool> Reload()
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

        public async Task<bool> Synchronise()
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

        public async Task<bool> Disconnect() => Connected && await AssociatedClient?.Disconnect();

        public override bool AddFunction(IFunction function) => false;
        public override bool CanBeTriggered(IEntity entity) => false;
        public override bool RemoveFunction(IFunction function) => false;
        public override void Stop() { }
        public override void Trigger(IEntity triggeredBy) { }

        private void UpdateProgramId(Guid programId)
        {
            if (programId != Guid.Empty)
            {

            }
        }
    }
}