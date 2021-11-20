using InDoOut_Core.Entities.Functions;
using InDoOut_Networking.Shared.Entities;
using System;

namespace InDoOut_Networking.Entities
{
    public interface INetworkedResult : IResult
    {
        DateTime LastUpdateTime { get; }

        bool UpdateFromStatus(ResultStatus status);
    }
}
