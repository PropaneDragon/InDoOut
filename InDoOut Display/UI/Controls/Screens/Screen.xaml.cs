using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Variables;
using InDoOut_Display.Actions;
using InDoOut_Display.Actions.Selecting;
using InDoOut_Display.UI.Controls.DisplayElement;
using InDoOut_Display_Core.Elements;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Deleting;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
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
        private readonly ScreenSelectionManager _selectionManager = null;

        private ProgramViewMode _currentViewMode = ProgramViewMode.IO;

        public Size Size => new Size(Width, Height);
        public ProgramViewMode CurrentViewMode { get => _currentViewMode; set => ChangeMode(value); }
        public IProgram AssociatedProgram { get; set; } = null;
        public ISelectionManager<ISelectable> SelectionManager => _selectionManager;
        public List<FrameworkElement> Elements => Grid_Elements.Children.Cast<FrameworkElement>().ToList();

        public Screen()
        {
            InitializeComponent();

            _actionHandler = new ActionHandler(new ScreenRestingAction(this));
            _selectionManager = new ScreenSelectionManager(this);
        }

        public bool AddDisplayElement(IDisplayElement displayElement)
        {
            if (displayElement != null && AssociatedProgram != null)
            {
                if (AssociatedProgram.Functions.Contains(displayElement.AssociatedElementFunction) || AssociatedProgram.AddFunction(displayElement.AssociatedElementFunction))
                {
                    var host = new DisplayElementContainer(displayElement) { ViewMode = CurrentViewMode };
                    _ = Grid_Elements.Children.Add(host);
                    _ = SelectionManager?.Set(host);

                    return true;
                }
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

        public void Add(FrameworkElement element)
        {
            throw new System.NotImplementedException();
        }

        public void Add(FrameworkElement element, Point position, int zIndex = 0)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(FrameworkElement element)
        {
            throw new System.NotImplementedException();
        }

        public void SetPosition(FrameworkElement element, Point position)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(IDeletable deletable)
        {
            throw new System.NotImplementedException();
        }

        public Point GetPosition(FrameworkElement element)
        {
            throw new System.NotImplementedException();
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

        private void ChangeMode(ProgramViewMode mode)
        {
            if (mode != _currentViewMode)
            {
                _currentViewMode = mode;

                var elements = Elements;

                foreach (var element in elements)
                {
                    if (element is IDisplayElementContainer elementContainer)
                    {
                        elementContainer.ViewMode = mode;
                    }
                }
            }
        }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _ = _actionHandler?.MouseLeftMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                _ = _actionHandler?.MouseRightMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
            else
            {
                _ = _actionHandler?.MouseNoMove(e.GetPosition(sender as IInputElement)) ?? false;
            }

            e.Handled = false;
        }

        private void UserControl_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement)) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _ = _actionHandler?.KeyDown(e.Key) ?? false;

            e.Handled = false;
        }

        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _ = _actionHandler?.KeyUp(e.Key) ?? false;

            e.Handled = false;
        }
    }
}
