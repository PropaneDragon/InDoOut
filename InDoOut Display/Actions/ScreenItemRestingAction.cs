using InDoOut_Display.UI.Controls.Screens;
using InDoOut_UI_Common.Actions;
using System;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions
{
    internal class ScreenItemRestingAction : InDoOut_UI_Common.Actions.Action
    {
        public ScreenItem ScreenItem { get; private set; } = null;

        public ScreenItemRestingAction(ScreenItem screenItem)
        {
            ScreenItem = screenItem;
        }

        public override bool MouseNoMove(Point mousePosition)
        {
            var position = Mouse.GetPosition(ScreenItem);
            var edge = ScreenItem?.GetCloseEdge(position) ?? ScreenItemEdge.None;

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
                    Finish(new ScreenItemResizeAction(ScreenItem, mousePosition));
                }
            }

            return false;
        }

        private Cursor GetCursorForEdge(ScreenItemEdge edge)
        {
            return edge switch
            {
                ScreenItemEdge.Left => Cursors.SizeWE,
                ScreenItemEdge.Right => Cursors.SizeWE,

                ScreenItemEdge.Top => Cursors.SizeNS,
                ScreenItemEdge.Bottom => Cursors.SizeNS,
                
                ScreenItemEdge.TopLeft => Cursors.SizeNWSE,
                ScreenItemEdge.BottomRight => Cursors.SizeNWSE,

                ScreenItemEdge.TopRight => Cursors.SizeNESW,
                ScreenItemEdge.BottomLeft => Cursors.SizeNESW,

                _ => null
            };
        }
    }
}
