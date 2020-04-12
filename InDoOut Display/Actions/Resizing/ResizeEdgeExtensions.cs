using InDoOut_Display_Core.Actions.Resizing;
using System.Collections.Generic;

namespace InDoOut_Display.Actions.Resizing
{
    public static class ResizeEdgeExtensions
    {
        public static bool ValidEdge(this ResizeEdge resizeEdge) => resizeEdge != ResizeEdge.None;

        public static bool IsCorner(this ResizeEdge resizeEdge)
        {
            switch (resizeEdge)
            {
                case ResizeEdge.BottomLeft:
                case ResizeEdge.BottomRight:
                case ResizeEdge.TopLeft:
                case ResizeEdge.TopRight:
                    return true;
            }

            return false;
        }

        public static ResizeEdge OppositeEdge(this ResizeEdge resizeEdge)
        {
            return resizeEdge switch
            {
                ResizeEdge.Bottom => ResizeEdge.Top,
                ResizeEdge.Left => ResizeEdge.Right,
                ResizeEdge.Right => ResizeEdge.Left,
                ResizeEdge.Top => ResizeEdge.Bottom,

                ResizeEdge.BottomLeft => ResizeEdge.TopRight,
                ResizeEdge.BottomRight => ResizeEdge.TopLeft,
                ResizeEdge.TopLeft => ResizeEdge.BottomRight,
                ResizeEdge.TopRight => ResizeEdge.BottomLeft,

                _ => ResizeEdge.None
            };
        }

        public static List<ResizeEdge> IndividualEdges(this ResizeEdge resizeEdge)
        {
            switch (resizeEdge)
            {
                case ResizeEdge.Bottom:
                case ResizeEdge.Left:
                case ResizeEdge.Right:
                case ResizeEdge.Top:
                    return new List<ResizeEdge>() { resizeEdge };
                case ResizeEdge.BottomLeft:
                    return new List<ResizeEdge>() { ResizeEdge.Bottom, ResizeEdge.Left };
                case ResizeEdge.BottomRight:
                    return new List<ResizeEdge>() { ResizeEdge.Bottom, ResizeEdge.Right };
                case ResizeEdge.TopLeft:
                    return new List<ResizeEdge>() { ResizeEdge.Top, ResizeEdge.Left };
                case ResizeEdge.TopRight:
                    return new List<ResizeEdge>() { ResizeEdge.Top, ResizeEdge.Right };
            }

            return new List<ResizeEdge>();
        }
    }
}
