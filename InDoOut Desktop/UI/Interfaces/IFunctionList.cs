using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;

namespace InDoOut_Desktop.UI.Interfaces
{
    internal interface IFunctionList
    {
        List<IFunction> Functions { get; }

        void Filter(string filter);
        void ClearFilter();
    }
}
