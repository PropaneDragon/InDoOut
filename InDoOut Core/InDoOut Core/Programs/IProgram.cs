using InDoOut_Core.Core;
using InDoOut_Core.Functions;
using System.Collections.Generic;

namespace InDoOut_Core.Programs
{
    /// <summary>
    /// Represents a group of self contained functions that have
    /// interactivity between each other.
    /// </summary>
    interface IProgram : ITriggerable<IEntity>
    {
        List<IFunction> Functions { get; }
    }
}
