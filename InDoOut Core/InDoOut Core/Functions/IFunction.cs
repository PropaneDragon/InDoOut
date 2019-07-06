using System;
using System.Collections.Generic;

namespace InDoOut_Core.Functions
{
    enum State
    {
        Placing,
        Waiting,
        Processing,
        InError
    }

    interface IFunction
    {
        string Name { get; }
        Guid Id { get; }
        State State { get; }

        List<IInput> Inputs { get; }
        List<IOutput> Outputs { get; }

        void Trigger(IInput input);
    }
}
