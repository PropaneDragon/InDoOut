using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.UI.Controls.Screens;
using InDoOut_UI_Common.Actions.Dragging;
using InDoOut_UI_Common.Actions.Selecting;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions
{
    internal class ScreenRestingAction : InDoOut_UI_Common.Actions.Action
    {
        private static readonly int RESIZE_EDGE_SENSITIVITY = 8;

        private readonly IScreen _screen = null;

        public ScreenRestingAction(IScreen screen)
        {
            _screen = screen;
        }

        public override bool MouseNoMove(Point mousePosition)
        {
            var topElements = _screen?.GetElementsUnderMouse();
            if (topElements != null)
            {
                var resizable = _screen?.GetFirstElementOfType<IResizable>(topElements);
                if (resizable != null && resizable.CanResize(_screen) && resizable.CloseToEdge(_screen, _screen.GetMousePosition(), RESIZE_EDGE_SENSITIVITY))
                {
                    var edge = resizable.GetCloseEdge(_screen, _screen.GetMousePosition(), RESIZE_EDGE_SENSITIVITY);
                    Mouse.OverrideCursor = GetCursorForEdge(edge);
                }
                else
                {
                    Mouse.OverrideCursor = null;
                }
            }

            return base.MouseNoMove(mousePosition);
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            var topElements = _screen?.GetElementsUnderMouse();
            var selectionManager = _screen?.SelectionManager;
            var elementsSelected = selectionManager?.Selection;

            if (topElements != null && selectionManager != null)
            {
                var resizable = _screen?.GetFirstElementOfType<IResizable>(topElements);
                var draggable = _screen?.GetFirstElementOfType<IDraggable>(topElements);

                if (resizable != null && resizable.CloseToEdge(_screen, _screen.GetMousePosition(), RESIZE_EDGE_SENSITIVITY) && elementsSelected.Any(selected => selected is IResizable resizable && resizable.CanResize(_screen)))
                {
                    var resizables = elementsSelected.Where(selected => selected is IResizable resizable && resizable.CanResize(_screen)).Cast<IResizable>();
                    var edge = resizable.GetCloseEdge(_screen, _screen.GetMousePosition(), RESIZE_EDGE_SENSITIVITY);

                    Finish(new ResizableResizeAction(_screen, resizables, edge, mousePosition));
                }
                else if (draggable != null && draggable.CanDrag(_screen))
                {
                    var draggables = elementsSelected.Where(selected => selected is IDraggable resizable && resizable.CanDrag(_screen)).Cast<IDraggable>();

                    Finish(new DraggableDragAction(_screen, draggables, mousePosition));
                }
            }

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            if (_screen != null)
            {
                var elementsUnderMouse = _screen.GetElementsUnderMouse();
                if (elementsUnderMouse.Count > 0)
                {
                    if (_screen.GetFirstElementOfType<ISelectable>(elementsUnderMouse) is ISelectable selectable && selectable.CanSelect(_screen))
                    {
                        _ = Keyboard.Modifiers.HasFlag(ModifierKeys.Control) ? _screen.SelectionManager.Add(selectable, true) : _screen.SelectionManager.Set(selectable, false);
                    }
                    else if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        _screen.SelectionManager.Clear();
                    }

                    return true;
                }
            }

            return false;
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
