﻿using InDoOut_Display.UI.Controls.Screens;
using System;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions.Resizing
{
    public class ScreenResizeAction : InDoOut_UI_Common.Actions.Dragging.DragAction
    {
        private Size _initialSize = new Size();
        private ScreenEdge _initialEdge = ScreenEdge.None;

        public Screen ScreenItem { get; private set; } = null;

        public ScreenResizeAction(Screen screenItem, Point mousePosition)
        {
            ScreenItem = screenItem;

            _ = base.MouseLeftDown(mousePosition);

            _initialSize = new Size(ScreenItem?.ActualWidth ?? 0, ScreenItem?.ActualHeight ?? 0);
            _initialEdge = ScreenItem?.GetCloseEdge(Mouse.GetPosition(screenItem)) ?? ScreenEdge.None;
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
                ScreenEdge.Left => 2,
                ScreenEdge.TopLeft => 2,
                ScreenEdge.BottomLeft => 2,
                ScreenEdge.Right => -2,
                ScreenEdge.TopRight => -2,
                ScreenEdge.BottomRight => -2,

                _ => 0
            };
        }

        private double CalculateVerticalMultiplier()
        {
            return _initialEdge switch
            {
                ScreenEdge.Bottom => -2,
                ScreenEdge.BottomLeft => -2,
                ScreenEdge.BottomRight => -2,
                ScreenEdge.Top => 2,
                ScreenEdge.TopLeft => 2,
                ScreenEdge.TopRight => 2,

                _ => 0
            };
        }
    }
}
