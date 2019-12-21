using InDoOut_Display.UI.Controls.Screens;
using System.Windows;

namespace InDoOut_Display.Actions.Resizing
{
    public class ResizableResizeAction : InDoOut_UI_Common.Actions.Dragging.DragAction
    {
        private readonly ResizeEdge _initialEdge = ResizeEdge.None;

        public IScreen AssociatedScreen { get; private set; } = null;
        public IResizable AssociatedResizable { get; private set; } = null;

        public ResizableResizeAction(IScreen screen, IResizable resizable, ResizeEdge edge, Point mousePosition)
        {
            AssociatedScreen = screen;
            AssociatedResizable = resizable;

            _ = MouseLeftDown(mousePosition);

            _initialEdge = edge;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            AssociatedResizable?.ResizeStarted(AssociatedScreen);

            return base.MouseLeftDown(mousePosition);
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            _ = base.MouseLeftMove(mousePosition);
            
            if (AssociatedResizable != null && AssociatedScreen != null && AssociatedResizable.CanResize(AssociatedScreen))
            {
                AssociatedResizable.SetEdgeToMouse(AssociatedScreen, _initialEdge);
            }

            return false;
        }

        public override bool MouseNoMove(Point mousePosition) => MouseLeftUp(mousePosition);
        public override bool MouseLeftUp(Point mousePosition)
        {
            AssociatedResizable?.ResizeEnded(AssociatedScreen);

            _ = base.MouseLeftUp(mousePosition);

            Finish(null);

            return true;
        }
    }
}
