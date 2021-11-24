using InDoOut_Display_Core.Actions.Resizing;
using System.Collections.Generic;

namespace InDoOut_Display.Actions.Resizing
{
    public static class ResizeEdgeExtensions
    {
        public static bool ValidEdge(this ResizeEdge resizeEdge) => resizeEdge != ResizeEdge.None;

        public static bool IsCorner(this ResizeEdge resizeEdge)
        {
            return resizeEdge switch
            {
                ResizeEdge.BottomLeft or ResizeEdge.BottomRight or ResizeEdge.TopLeft or ResizeEdge.TopRight => true,

                _ => false,
            };
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
            return resizeEdge switch
            {
                ResizeEdge.Bottom or ResizeEdge.Left or ResizeEdge.Right or ResizeEdge.Top => new List<ResizeEdge>() { resizeEdge },
                ResizeEdge.BottomLeft => new List<ResizeEdge>() { ResizeEdge.Bottom, ResizeEdge.Left },
                ResizeEdge.BottomRight => new List<ResizeEdge>() { ResizeEdge.Bottom, ResizeEdge.Right },
                ResizeEdge.TopLeft => new List<ResizeEdge>() { ResizeEdge.Top, ResizeEdge.Left },
                ResizeEdge.TopRight => new List<ResizeEdge>() { ResizeEdge.Top, ResizeEdge.Right },

                _ => new List<ResizeEdge>(),
            };
        }
    }
}
