﻿using InDoOut_Core.Entities.Programs;
using InDoOut_Networking.Client;
using System;
using System.Threading.Tasks;

namespace InDoOut_Networking.Entities
{
    public interface INetworkedProgram : IProgram
    {
        bool Connected { get; }

        IClient AssociatedClient { get; }

        Task<bool> Reload();
        Task<bool> Synchronise();
        Task<bool> Disconnect();
    }
}