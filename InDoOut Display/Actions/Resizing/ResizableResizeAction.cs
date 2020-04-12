using InDoOut_Display_Core.Actions.Resizing;
using InDoOut_Display_Core.Screens;
using System.Collections.Generic;
using System.Windows;

namespace InDoOut_Display.Actions.Resizing
{
    public class ResizableResizeAction : InDoOut_UI_Common.Actions.Dragging.DragAction
    {
        private readonly ResizeEdge _initialEdge = ResizeEdge.None;

        public IScreen AssociatedScreen { get; private set; } = null;
        public IEnumerable<IResizable> AssociatedResizables { get; private set; } = null;

        public ResizableResizeAction(IScreen screen, IEnumerable<IResizable> resizables, ResizeEdge edge, Point mousePosition)
        {
            AssociatedScreen = screen;
            AssociatedResizables = resizables;

            _ = MouseLeftDown(mousePosition);

            _initialEdge = edge;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            _ = base.MouseLeftDown(mousePosition);

            foreach (var resizable in AssociatedResizables)
            {
                resizable.ResizeStarted(AssociatedScreen);
            }

            return true;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            _ = base.MouseLeftMove(mousePosition);

            foreach (var resizable in AssociatedResizables)
            {
                if (resizable != null && AssociatedScreen != null && resizable.CanResize(AssociatedScreen))
                {
                    resizable.ResizeMoved(AssociatedScreen, _initialEdge, MouseDelta);
                }
            }

            return true;
        }

        public override bool MouseNoMove(Point mousePosition) => MouseLeftUp(mousePosition);
        public override bool MouseLeftUp(Point mousePosition)
        {
            _ = base.MouseLeftUp(mousePosition);

            foreach (var resizable in AssociatedResizables)
            {
                resizable.ResizeEnded(AssociatedScreen);
            }

            Finish(null);

            return true;
        }
    }
}
