using InDoOut_Display.Actions.Resizing;
using InDoOut_Display.UI.Controls.Screens;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions
{
    internal class ScreenOverviewRestingAction : InDoOut_UI_Common.Actions.Action
    {
        public Screen ScreenItem { get; private set; } = null;

        public ScreenOverviewRestingAction(Screen screenItem)
        {
            ScreenItem = screenItem;
        }

        public override bool MouseNoMove(Point mousePosition)
        {
            var position = Mouse.GetPosition(ScreenItem);
            var edge = ScreenItem?.GetCloseEdge(position, 5) ?? ScreenEdge.None;

            Mouse.OverrideCursor = GetCursorForEdge(edge);

            return false;
        }

        public override bool MouseLeftDown(Point mousePosition)
        {
            if (ScreenItem != null)
            {
                var position = Mouse.GetPosition(ScreenItem);
                if (ScreenItem?.PointCloseToScreenItemEdge(position) ?? false)
                {
                    Finish(new ScreenResizeAction(ScreenItem, mousePosition));
                }
            }

            return false;
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
