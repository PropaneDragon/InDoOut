using InDoOut_Core.Entities.Programs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core.Networking
{
    public interface IProgramSyncClient : IClient
    {
        Task<bool> SendProgram(IProgram program, CancellationToken cancellationToken);
        Task<List<string>> RequestAvailablePrograms(CancellationToken cancellationToken);
    }
}