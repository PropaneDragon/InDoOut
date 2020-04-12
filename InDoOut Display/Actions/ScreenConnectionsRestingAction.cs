using InDoOut_Display.Actions.Resizing;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Actions;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions
{
    internal class ScreenConnectionsRestingAction : Action
    {
        private readonly ActionHandler _commonDisplayActions;

        public IScreenConnections ScreenConnections { get; private set; } = null;

        private ScreenConnectionsRestingAction()
        {
        }

        public ScreenConnectionsRestingAction(IScreenConnections screenItem) : this()
        {
            ScreenConnections = screenItem;

            _commonDisplayActions = new ActionHandler(new CommonProgramDisplayRestingAction(ScreenConnections));
        }

        public override bool MouseNoMove(Point mousePosition)
        {
            if (ScreenConnections?.CurrentScreen != null && ScreenConnections?.CurrentScreen is FrameworkElement element)
            {
                var position = Mouse.GetPosition(element);
                var edge = ScreenConnections?.CurrentScreen?.GetCloseEdge(position, 5) ?? ScreenEdge.None;

                Mouse.OverrideCursor = GetCursorForEdge(edge);

                if (edge != ScreenEdge.None)
                {
                    return true;
                }
            }

            return _commonDisplayActions?.MouseNoMove(mousePosition) ?? false;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            if (ScreenConnections?.CurrentScreen != null && ScreenConnections?.CurrentScreen is FrameworkElement element)
            {
                var position = Mouse.GetPosition(element);
                if (ScreenConnections?.CurrentScreen?.PointCloseToScreenItemEdge(position) ?? false)
                {
                    Finish(new ScreenResizeAction(ScreenConnections, mousePosition));

                    return true;
                }
            }

            return _commonDisplayActions?.MouseLeftDown(mousePosition) ?? false;
        }

        public override bool MouseDoubleClick(Point mousePosition)
        {
            return _commonDisplayActions?.MouseDoubleClick(mousePosition) ?? false;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            return _commonDisplayActions?.MouseLeftMove(mousePosition) ?? false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            return _commonDisplayActions?.MouseLeftUp(mousePosition) ?? false;
        }

        public override bool MouseRightDown(Point mousePosition)
        {
            return _commonDisplayActions?.MouseRightDown(mousePosition) ?? false;
        }

        public override bool MouseRightMove(Point mousePosition)
        {
            return _commonDisplayActions?.MouseRightMove(mousePosition) ?? false;
        }

        public override bool MouseRightUp(Point mousePosition)
        {
            return _commonDisplayActions?.MouseRightUp(mousePosition) ?? false;
        }

        public override bool KeyDown(Key key)
        {
            return _commonDisplayActions?.KeyDown(key) ?? false;
        }

        public override bool KeyUp(Key key)
        {
            return _commonDisplayActions?.KeyUp(key) ?? false;
        }

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
