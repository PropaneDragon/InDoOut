using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.UI.Controls.Screens;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions
{
    internal class ScreenRestingAction : InDoOut_UI_Common.Actions.Action
    {
        private static readonly int RESIZE_EDGE_SENSITIVITY = 6;

        private IScreen _screen = null;

        public ScreenRestingAction(IScreen screen)
        {
            _screen = screen;
        }

        public override bool MouseNoMove(Point mousePosition)
        {
            var topElement = _screen?.GetElementUnderMouse();
            if (topElement != null)
            {
                var resizable = _screen?.GetFirstElementOfType<IResizable>(topElement);
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
