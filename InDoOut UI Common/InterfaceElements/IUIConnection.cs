using InDoOut_UI_Common.Actions.Deleting;
using InDoOut_UI_Common.Actions.Selecting;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIConnection : ISelectable, IDeletable
    {
        bool Hidden { get; set; }

        IUIConnectionStart AssociatedStart { get; set; }
        IUIConnectionEnd AssociatedEnd { get; set; }

        Point Start { get; set; }
        Point End { get; set; }

        void UpdatePositionFromInputOutput(IElementDisplay view);
    }
}
