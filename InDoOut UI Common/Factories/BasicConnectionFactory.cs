using InDoOut_UI_Common.Controls.CoreEntityRepresentation;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Factories
{
    public class BasicConnectionFactory : AbstractElementFactory, IConnectionFactory
    {
        protected ICommonProgramDisplay Display { get; private set; } = null;

        private BasicConnectionFactory()
        {
        }

        public BasicConnectionFactory(ICommonProgramDisplay display) : this()
        {
            Display = display;
        }

        public virtual IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end)
        {
            if (Display != null && start != null && end != null && end is FrameworkElement element)
            {
                var endPosition = Display.GetPosition(element);
                var uiConnection = Create(start, endPosition);

                if (uiConnection != null)
                {
                    uiConnection.AssociatedEnd = end;

                    return uiConnection;
                }
            }

            return null;
        }

        public virtual IUIConnection Create(IUIConnectionStart start, Point end)
        {
            if (Display != null && start != null && start is FrameworkElement element)
            {
                var bestSidePoint = Display.GetBestSide(element, end);
                var uiConnection = new UIConnection()
                {
                    Start = bestSidePoint,
                    End = end,
                    AssociatedStart = start
                };

                Display.Add(uiConnection, new Point(0, 0), -999);

                return uiConnection;
            }

            return null;
        }
    }
}
