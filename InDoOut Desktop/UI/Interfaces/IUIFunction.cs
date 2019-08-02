using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIFunction
    {
        IFunction AssociatedFunction { get; set; }
        List<IUIInput> Inputs { get; }
        List<IUIOutput> Outputs { get; }
    }
}
