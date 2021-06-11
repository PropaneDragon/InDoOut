using InDoOut_Display.Actions.Resizing;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Actions;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions
{
    internal class ScreenConnectionsRestingAction : CommonProgramDisplayRestingAction
    {
        public ScreenConnectionsRestingAction(IScreenConnections screenConnections) : base(screenConnections)
        {
        }

        public override bool MouseNoMove(Point mousePosition)
        {
            if (Display is IScreenConnections screenConnections && screenConnections?.CurrentScreen != null && screenConnections?.CurrentScreen is FrameworkElement element)
            {
                var position = Mouse.GetPosition(element);
                var edge = screenConnections?.CurrentScreen?.GetCloseEdge(position, 5) ?? ScreenEdge.None;

                Mouse.OverrideCursor = GetCursorForEdge(edge);

                if (edge != ScreenEdge.None)
                {
                    return true;
                }
            }

            return base.MouseNoMove(mousePosition);
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            if (Display is IScreenConnections screenConnections && screenConnections?.CurrentScreen != null && screenConnections?.CurrentScreen is FrameworkElement element)
            {
                var position = Mouse.GetPosition(element);
                if (screenConnections?.CurrentScreen?.PointCloseToScreenItemEdge(position) ?? false)
                {
                    Finish(new ScreenResizeAction(screenConnections, mousePosition));

                    return true;
                }
            }

            return base.MouseLeftDown(mousePosition);
        }

        public override bool MouseDoubleClick(Point mousePosition) => base.MouseDoubleClick(mousePosition);

        public override bool MouseLeftMove(Point mousePosition) => base.MouseLeftMove(mousePosition);

        public override bool MouseLeftUp(Point mousePosition) => base.MouseLeftUp(mousePosition);

        public override bool MouseRightDown(Point mousePosition) => base.MouseRightDown(mousePosition);

        public override bool MouseRightMove(Point mousePosition) => base.MouseRightMove(mousePosition);

        public override bool MouseRightUp(Point mousePosition) => base.MouseRightUp(mousePosition);

        public override bool KeyDown(Key key) => base.KeyDown(key);

        public override bool KeyUp(Key key) => base.KeyUp(key);

        private Cursor GetCursorForEdge(ScreenEdge edge)
        {
            return edge switch
            {
                ScreenEdge.Left => Cursors.SizeWE,
                ScreenEdge.Right => Cursors.SizeWE,

                ScreenEdge.Top => Cursors.SizeNS,
                ScreenEdge.Bottom => Cursors.SizeNS,

                ScreenEdge.TopLeft => Cursors.SizeNWSE,
                ScreenEdge.BottomRight => Cursors.SizeNWSE,

                ScreenEdge.TopRight => Cursors.SizeNESW,
                ScreenEdge.BottomLeft => Cursors.SizeNESW,

                _ => null
            };
        }
    }
}
