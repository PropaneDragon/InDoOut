using InDoOut_Core.Entities.Functions;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIConnection
    {
        IUIOutput AssociatedOutput { get; set; }
        IUIInput AssociatedInput { get; set; }

        Point Start { get; set; }
        Point End { get; set; }
    }
}
