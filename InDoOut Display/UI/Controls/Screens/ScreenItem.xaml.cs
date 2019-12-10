﻿using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class ScreenItem : UserControl, IScreenItem
    {
        public ScreenItem()
        {
            InitializeComponent();
        }

        public ScreenItemEdge GetCloseEdge(Point point, double distance = 5d)
        {
            var size = new Size(ActualWidth, ActualHeight);
            var nearLeft = PointWithin(point.X, -distance, distance);
            var nearTop = PointWithin(point.Y, -distance, distance);
            var nearRight = PointWithin(point.X, size.Width - distance, size.Width + distance);
            var nearBottom = PointWithin(point.Y, size.Height - distance, size.Height + distance);

            if (nearLeft)
            {
                return nearTop ? ScreenItemEdge.TopLeft : nearBottom ? ScreenItemEdge.BottomLeft : ScreenItemEdge.Left;
            }
            else if (nearRight)
            {
                return nearTop ? ScreenItemEdge.TopRight : nearBottom ? ScreenItemEdge.BottomRight : ScreenItemEdge.Right;
            }
            else if (nearBottom)
            {
                return ScreenItemEdge.Bottom;
            }
            else if (nearTop)
            {
                return ScreenItemEdge.Top;
            }

            return ScreenItemEdge.None;
        }

        public bool PointCloseToScreenItemEdge(Point point, double distance = 5d)
        {
            return GetCloseEdge(point, distance) != ScreenItemEdge.None;
        }

        private bool PointWithin(double point, double min, double max)
        {
            return point > min && point < max;
        }
    }
}