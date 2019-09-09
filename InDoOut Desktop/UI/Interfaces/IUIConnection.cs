using InDoOut_Desktop.Actions.Deleting;
using InDoOut_Desktop.Actions.Selecting;
using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIConnection : ISelectable, IDeletable
    {
        bool Hidden { get; set; }

        IUIConnectionStart AssociatedStart { get; set; }
        IUIConnectionEnd AssociatedEnd { get; set; }

        Point Start { get; set; }
        Point End { get; set; }

        void UpdatePositionFromInputOutput(IElementDisplay display);
    }
}
