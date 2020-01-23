using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IScrollable
    {
        Point Offset { get; set; }

        void MoveToCentre();
    }
}
