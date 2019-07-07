using InDoOut_Core.Entities.Core;
using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;

namespace InDoOut_Core.Entities.Programs
{
    /// <summary>
    /// Represents a group of self contained functions that have
    /// interactivity between each other.
    /// </summary>
    interface IProgram : ITriggerable<IEntity>, INamed, IStored
    {
        List<IFunction> Functions { get; }
    }
}
