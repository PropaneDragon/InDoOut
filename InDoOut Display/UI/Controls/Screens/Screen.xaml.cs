﻿using InDoOut_Display.Actions;
using InDoOut_Display_Core.Elements;
using InDoOut_UI_Common.Actions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class Screen : UserControl, IScreen
    {
        private readonly ActionHandler _actionHandler = null;

        public Screen()
        {
            InitializeComponent();

            _actionHandler = new ActionHandler(new ScreenRestingAction(this));
        }

        public bool AddDisplayElement(IDisplayElement displayElement)
        {
            if (displayElement != null && displayElement is UIElement uiElement)
            {
                _ = Grid_Elements.Children.Add(uiElement);
            }

            return false;
        }

        public bool RemoveDisplayElement(IDisplayElement displayElement)
        {
            if (displayElement != null)
            {

            }

            return false;
        }

        public ScreenItemEdge GetCloseEdge(Point point, double distance = 5d)
        {
            var size = new Size(ActualWidth, ActualHeight);
            var inBounds = point.X > -distance && point.X < (size.Width + distance) && point.Y > -distance && point.Y < (size.Height + distance);
            var nearLeft = inBounds && PointWithin(point.X, -distance, distance);
            var nearTop = inBounds && PointWithin(point.Y, -distance, distance);
            var nearRight = inBounds && PointWithin(point.X, size.Width - distance, size.Width + distance);
            var nearBottom = inBounds && PointWithin(point.Y, size.Height - distance, size.Height + distance);

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

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var handled = false;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                handled = _actionHandler?.MouseLeftMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                handled = _actionHandler?.MouseRightMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
            else
            {
                handled = _actionHandler?.MouseNoMove(e.GetPosition(sender as IInputElement)) ?? false;
            }

            e.Handled = handled;
        }

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void UserControl_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void UserControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = _actionHandler?.KeyDown(e.Key) ?? false;
        }

        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = _actionHandler?.KeyUp(e.Key) ?? false;
        }
    }
}
