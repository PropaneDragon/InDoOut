using System.Windows;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIConnection
    {
        bool Hidden { get; set; }

        IUIConnectionStart AssociatedStart { get; set; }
        IUIConnectionEnd AssociatedEnd { get; set; }

        Point Start { get; set; }
        Point End { get; set; }

        void UpdatePositionFromInputOutput(IElementDisplay display);
    }
}
