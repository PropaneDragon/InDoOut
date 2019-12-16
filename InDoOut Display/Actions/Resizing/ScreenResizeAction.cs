using InDoOut_Display.UI.Controls.Screens;
using System;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions.Resizing
{
    public class ScreenItemResizeAction : InDoOut_UI_Common.Actions.Dragging.DragAction
    {
        private Size _initialSize = new Size();
        private ScreenItemEdge _initialEdge = ScreenItemEdge.None;

        public Screen ScreenItem { get; private set; } = null;

        public ScreenItemResizeAction(Screen screenItem, Point mousePosition)
        {
            ScreenItem = screenItem;

            _ = base.MouseLeftDown(mousePosition);

            _initialSize = new Size(ScreenItem?.ActualWidth ?? 0, ScreenItem?.ActualHeight ?? 0);
            _initialEdge = ScreenItem?.GetCloseEdge(Mouse.GetPosition(screenItem)) ?? ScreenItemEdge.None;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            _ = base.MouseLeftMove(mousePosition);

            var movementAmount = CalculateMovementAmount();

            if (ScreenItem != null)
            {
                ScreenItem.Width = Math.Clamp(_initialSize.Width + movementAmount.X, 10d, double.MaxValue);
                ScreenItem.Height = Math.Clamp(_initialSize.Height + movementAmount.Y, 10d, double.MaxValue);

                return true;
            }

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            _ = base.MouseLeftUp(mousePosition);

            Finish(null);

            return true;
        }

        private Point CalculateMovementAmount()
        {
            var horizontalMultiplier = CalculateHorizontalMultiplier();
            var verticalMultiplier = CalculateVerticalMultiplier();

            var horizontalMovement = MouseDelta.X * horizontalMultiplier;
            var verticalMovement = MouseDelta.Y * verticalMultiplier;

            return new Point(horizontalMovement, verticalMovement);
        }

        private double CalculateHorizontalMultiplier()
        {
            return _initialEdge switch
            {
                ScreenItemEdge.Left => 2,
                ScreenItemEdge.TopLeft => 2,
                ScreenItemEdge.BottomLeft => 2,
                ScreenItemEdge.Right => -2,
                ScreenItemEdge.TopRight => -2,
                ScreenItemEdge.BottomRight => -2,

                _ => 0
            };
        }

        private double CalculateVerticalMultiplier()
        {
            return _initialEdge switch
            {
                ScreenItemEdge.Bottom => -2,
                ScreenItemEdge.BottomLeft => -2,
                ScreenItemEdge.BottomRight => -2,
                ScreenItemEdge.Top => 2,
                ScreenItemEdge.TopLeft => 2,
                ScreenItemEdge.TopRight => 2,

                _ => 0
            };
        }
    }
}
