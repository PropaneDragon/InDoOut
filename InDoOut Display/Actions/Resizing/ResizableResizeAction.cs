﻿using InDoOut_Display.UI.Controls.Screens;
using System.Windows;

namespace InDoOut_Display.Actions.Resizing
{
    public class ResizableResizeAction : InDoOut_UI_Common.Actions.Dragging.DragAction
    {
        private readonly static double MINIMUM_SIZE = 5;
        private readonly Size _initialSize = new Size();
        private readonly ResizeEdge _initialEdge = ResizeEdge.None;

        public IScreen AssociatedScreen { get; private set; } = null;
        public IResizable AssociatedResizable { get; private set; } = null;

        public ResizableResizeAction(IScreen screen, IResizable resizable, ResizeEdge edge, Point mousePosition)
        {
            AssociatedScreen = screen;
            AssociatedResizable = resizable;

            _ = base.MouseLeftDown(mousePosition);

            _initialSize = AssociatedResizable?.Size ?? Size.Empty;
            _initialEdge = edge;
        }

        public override bool MouseLeftMove(Point mousePosition)
        {
            _ = base.MouseLeftMove(mousePosition);

            var resizeEdges = _initialEdge.IndividualEdges();

            foreach (var edge in resizeEdges)
            {
                var distanceForEdge = GetDistanceForEdge(edge);
                distanceForEdge = distanceForEdge < MINIMUM_SIZE ? MINIMUM_SIZE : distanceForEdge;

                if (AssociatedResizable != null && AssociatedScreen != null && AssociatedResizable.CanResize(AssociatedScreen))
                {
                    AssociatedResizable.SetEdgeDistance(AssociatedScreen, edge, distanceForEdge);
                }
            }

            return false;
        }

        private double GetDistanceForEdge(ResizeEdge edge)
        {
            var adjustedDelta = new Point(MouseDelta.X * (edge == ResizeEdge.Left ? 1 : -1), MouseDelta.Y * (edge == ResizeEdge.Top ? 1 : -1));
            var adjustedSize = new Size(_initialSize.Width + adjustedDelta.X, _initialSize.Height + adjustedDelta.Y);

            switch (edge)
            {
                case ResizeEdge.Bottom:
                case ResizeEdge.Top:
                    return adjustedSize.Height;
                case ResizeEdge.Left:
                case ResizeEdge.Right:
                    return adjustedSize.Width;
            }

            return 0;
        }

        public override bool MouseNoMove(Point mousePosition) => MouseLeftUp(mousePosition);
        public override bool MouseLeftUp(Point mousePosition)
        {
            _ = base.MouseLeftUp(mousePosition);

            Finish(null);

            return true;
        }
    }
}
