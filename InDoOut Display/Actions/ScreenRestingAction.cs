using InDoOut_Display.Actions.Resizing;
using InDoOut_Display_Core.Actions.Resizing;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Actions;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions
{
    internal class ScreenRestingAction : CommonDisplayRestingAction
    {
        private static readonly int RESIZE_EDGE_SENSITIVITY = 8;

        public IScreen Screen => Display as IScreen;

        public ScreenRestingAction(IScreen screen) : base(screen)
        {
        }

        public override bool MouseNoMove(Point mousePosition)
        {
            var topElements = Display?.GetElementsUnderMouse();
            if (topElements != null)
            {
                var resizable = Display?.GetFirstElementOfType<IResizable>(topElements);
                if (resizable != null && Screen != null && resizable.CanResize(Screen) && resizable.CloseToEdge(Screen, Display.GetMousePosition(), RESIZE_EDGE_SENSITIVITY))
                {
                    var edge = resizable.GetCloseEdge(Screen, Display.GetMousePosition(), RESIZE_EDGE_SENSITIVITY);
                    Mouse.OverrideCursor = GetCursorForEdge(edge);
                }
                else
                {
                    Mouse.OverrideCursor = null;
                }
            }

            return base.MouseNoMove(mousePosition);
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            var topElements = Display?.GetElementsUnderMouse();
            var selectionManager = Display?.SelectionManager;
            var elementsSelected = selectionManager?.Selection;

            if (topElements != null && selectionManager != null)
            {
                var resizable = Display?.GetFirstElementOfType<IResizable>(topElements);

                if (resizable != null && Screen != null && resizable.CloseToEdge(Screen, Display.GetMousePosition(), RESIZE_EDGE_SENSITIVITY) && elementsSelected.Any(selected => selected is IResizable resizable && resizable.CanResize(Screen)))
                {
                    var resizables = elementsSelected.Where(selected => selected is IResizable resizable && resizable.CanResize(Screen)).Cast<IResizable>();
                    var edge = resizable.GetCloseEdge(Screen, Display.GetMousePosition(), RESIZE_EDGE_SENSITIVITY);

                    Finish(new ResizableResizeAction(Screen, resizables, edge, mousePosition));

                    return true;
                }
            }

            return base.MouseLeftMove(mousePosition);
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            return base.MouseLeftUp(mousePosition);
        }

        private Cursor GetCursorForEdge(ResizeEdge edge)
        {
            return edge switch
            {
                ResizeEdge.Left => Cursors.SizeWE,
                ResizeEdge.Right => Cursors.SizeWE,

                ResizeEdge.Top => Cursors.SizeNS,
                ResizeEdge.Bottom => Cursors.SizeNS,

                ResizeEdge.TopLeft => Cursors.SizeNWSE,
                ResizeEdge.BottomRight => Cursors.SizeNWSE,

                ResizeEdge.TopRight => Cursors.SizeNESW,
                ResizeEdge.BottomLeft => Cursors.SizeNESW,

                _ => null
            };
        }
    }
}
