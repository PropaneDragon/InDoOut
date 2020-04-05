using InDoOut_Display.UI.Controls.Screens;
using InDoOut_UI_Common.Actions.Dragging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InDoOut_Display.Actions.Resizing
{
    public class ScreenResizeAction : InDoOut_UI_Common.Actions.Dragging.DragAction
    {
        private Size _initialSize = new Size();
        private ScreenEdge _initialEdge = ScreenEdge.None;
        private IEnumerable<IDraggable> _cachedElements;

        public IScreenConnections ScreenConnections { get; private set; } = null;

        public ScreenResizeAction(IScreenConnections screenConnections, Point mousePosition)
        {
            ScreenConnections = screenConnections;

            var screen = ScreenConnections?.CurrentScreen;

            _ = base.MouseLeftDown(mousePosition);

            if (screen is FrameworkElement element)
            {
                _initialSize = new Size(element?.ActualWidth ?? 0, element?.ActualHeight ?? 0);
                _initialEdge = screen?.GetCloseEdge(Mouse.GetPosition(element)) ?? ScreenEdge.None;
                _cachedElements = screen?.Elements?.Where(element => element is IDraggable).Cast<IDraggable>();

                foreach (var cachedElement in _cachedElements)
                {
                    cachedElement.DragStarted(ScreenConnections);
                }
            }
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            _ = base.MouseLeftMove(mousePosition);

            var screen = ScreenConnections?.CurrentScreen;
            var movementAmount = CalculateMovementAmount();

            if (screen != null && screen is FrameworkElement element)
            {
                element.Width = Math.Clamp(_initialSize.Width + movementAmount.X, 10d, double.MaxValue);
                element.Height = Math.Clamp(_initialSize.Height + movementAmount.Y, 10d, double.MaxValue);

                if (_cachedElements != null)
                {
                    foreach (var cachedElement in _cachedElements)
                    {
                        cachedElement.DragMoved(ScreenConnections, MouseDelta);
                    }
                }

                return true;
            }

            return false;
        }

        public override bool MouseLeftUp(Point mousePosition)
        {
            _ = base.MouseLeftUp(mousePosition);

            if (_cachedElements != null)
            {
                foreach (var cachedElement in _cachedElements)
                {
                    cachedElement.DragEnded(ScreenConnections);
                }
            }

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
