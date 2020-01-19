using InDoOut_Display.Actions;
using InDoOut_Display.Actions.Selecting;
using InDoOut_Display.UI.Controls.DisplayElement;
using InDoOut_Display_Core.Elements;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class Screen : UserControl, IScreen
    {
        private readonly ActionHandler _actionHandler = null;
        private readonly SelectionManager _selectionManager = null;

        private ScreenMode _mode = ScreenMode.Layout;

        public Size Size => new Size(Width, Height);

        public ScreenMode Mode { get => _mode; set => ChangeMode(value); }

        public ISelectionManager<IScreenSelectable> SelectionManager => _selectionManager;

        public Screen()
        {
            InitializeComponent();

            _actionHandler = new ActionHandler(new ScreenRestingAction(this));
            _selectionManager = new SelectionManager(this);
        }

        public bool AddDisplayElement(IDisplayElement displayElement)
        {
            if (displayElement != null)
            {
                var host = new DisplayElementContainer(displayElement);
                _ = Grid_Elements.Children.Add(host);

                return true;
            }

            return false;
        }

        public bool RemoveDisplayElement(IDisplayElement displayElement)
        {
            if (displayElement != null)
            {
                //Todo: Remove elements
            }

            return false;
        }

        public Point GetMousePosition()
        {
            return Mouse.GetPosition(this);
        }

        public FrameworkElement GetElementUnderMouse()
        {
            return GetElementAtPoint(GetMousePosition());
        }

        public FrameworkElement GetElementAtPoint(Point point)
        {
            return GetElementsAtPoint(point).FirstOrDefault();
        }

        public List<FrameworkElement> GetElementsUnderMouse()
        {
            return GetElementsAtPoint(GetMousePosition());
        }

        public List<FrameworkElement> GetElementsAtPoint(Point point)
        {
            var hits = new List<FrameworkElement>();

            VisualTreeHelper.HitTest(Grid_Elements, FilterHit, (result) => NewHit(result, hits), new PointHitTestParameters(point));

            hits.AddRange(new List<FrameworkElement>() { Grid_Elements, this });
            return hits;
        }

        public T GetFirstElementOfType<T>(FrameworkElement element) where T : class
        {
            if (element != null)
            {
                if (typeof(T).IsAssignableFrom(element.GetType()) && element is T converted)
                {
                    return converted;
                }
                else
                {
                    var parent = VisualTreeHelper.GetParent(element);
                    return GetFirstElementOfType<T>(parent as FrameworkElement);
                }
            }

            return null;
        }

        public T GetFirstElementOfType<T>(List<FrameworkElement> elements) where T : class
        {
            foreach (var element in elements)
            {
                var foundElement = GetFirstElementOfType<T>(element);
                if (foundElement != null)
                {
                    return foundElement;
                }
            }

            return null;
        }

        public ScreenEdge GetCloseEdge(Point point, double distance = 5d)
        {
            var size = new Size(ActualWidth, ActualHeight);
            var inBounds = point.X > -distance && point.X < (size.Width + distance) && point.Y > -distance && point.Y < (size.Height + distance);
            var nearLeft = inBounds && PointWithin(point.X, -distance, distance);
            var nearTop = inBounds && PointWithin(point.Y, -distance, distance);
            var nearRight = inBounds && PointWithin(point.X, size.Width - distance, size.Width + distance);
            var nearBottom = inBounds && PointWithin(point.Y, size.Height - distance, size.Height + distance);

            if (nearLeft)
            {
                return nearTop ? ScreenEdge.TopLeft : nearBottom ? ScreenEdge.BottomLeft : ScreenEdge.Left;
            }
            else if (nearRight)
            {
                return nearTop ? ScreenEdge.TopRight : nearBottom ? ScreenEdge.BottomRight : ScreenEdge.Right;
            }
            else if (nearBottom)
            {
                return ScreenEdge.Bottom;
            }
            else if (nearTop)
            {
                return ScreenEdge.Top;
            }

            return ScreenEdge.None;
        }

        public bool PointCloseToScreenItemEdge(Point point, double distance = 5d)
        {
            return GetCloseEdge(point, distance) != ScreenEdge.None;
        }

        private HitTestFilterBehavior FilterHit(DependencyObject potentialHitTestTarget)
        {
            return potentialHitTestTarget is UIElement uiElement && uiElement.Visibility != Visibility.Visible
                ? HitTestFilterBehavior.ContinueSkipSelfAndChildren
                : HitTestFilterBehavior.Continue;
        }

        private HitTestResultBehavior NewHit(HitTestResult result, List<FrameworkElement> hits)
        {
            if (result.VisualHit != null && result.VisualHit is FrameworkElement element)
            {
                hits.Add(element);
            }

            return HitTestResultBehavior.Continue;
        }

        private bool PointWithin(double point, double min, double max)
        {
            return point > min && point < max;
        }

        private void ChangeMode(ScreenMode mode)
        {
            if (mode != _mode)
            {
                _mode = mode;
            }
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
