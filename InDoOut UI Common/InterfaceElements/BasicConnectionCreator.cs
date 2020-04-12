using InDoOut_UI_Common.Controls.CoreEntityRepresentation;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public class BasicConnectionCreator : AbstractCreator, IConnectionCreator
    {
        public virtual IUIConnection Create(ICommonProgramDisplay display, IUIConnectionStart start, IUIConnectionEnd end)
        {
            if (display != null && start != null && end != null && end is FrameworkElement element)
            {
                var endPosition = display.GetPosition(element);
                var uiConnection = Create(display, start, endPosition);

                if (uiConnection != null)
                {
                    uiConnection.AssociatedEnd = end;

                    return uiConnection;
                }
            }

            return null;
        }

        public virtual IUIConnection Create(ICommonProgramDisplay display, IUIConnectionStart start, Point end)
        {
            if (display != null && start != null && start is FrameworkElement element)
            {
                var bestSidePoint = display.GetBestSide(element, end);
                var uiConnection = new UIConnection()
                {
                    Start = bestSidePoint,
                    End = end,
                    AssociatedStart = start
                };

                display.Add(uiConnection, new Point(0, 0), -999);

                return uiConnection;
            }

            return null;
        }
    }
}
