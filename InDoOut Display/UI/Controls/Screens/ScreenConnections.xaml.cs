using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Display.Actions;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Deleting;
using InDoOut_UI_Common.Controls.CoreEntityRepresentation;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class ScreenConnections : UserControl, IScreenConnections
    {
        private readonly ActionHandler _actionHandler = null;
        private IProgram _currentProgram = null;

        public IScreen CurrentScreen => ScreenItem_Overview;

        public List<FrameworkElement> Elements => throw new System.NotImplementedException();

        public List<IUIFunction> UIFunctions => throw new System.NotImplementedException();

        public List<IUIConnection> UIConnections => throw new System.NotImplementedException();

        public IProgram AssociatedProgram { get => _currentProgram; set => ChangeProgram(value); }

        public ScreenConnections() : this(new Program()) //Todo: Program manager
        {

        }   
        
        public ScreenConnections(IProgram program)
        {
            InitializeComponent();

            _actionHandler = new ActionHandler(new ScreenConnectionsRestingAction(ScreenItem_Overview));

            ChangeProgram(program);
            //ChangeViewMode(CurrentViewMode); Todo: View mode
        }

        public void Add(FrameworkElement element)
        {
            //Todo: An actual relevant point. Not just 200, 200
            Add(element, new Point(200, 200));
        }

        public void Add(FrameworkElement element, Point position, int zIndex = 0)
        {
            if (element != null)
            {
                _ = Canvas_Content.Children.Add(element);

                Panel.SetZIndex(element, zIndex);

                SetPosition(element, position);
                //ChangeViewMode(CurrentViewMode); Todo: View mode
            }
        }

        public void Remove(FrameworkElement element)
        {
            throw new System.NotImplementedException();
        }

        public void SetPosition(FrameworkElement element, Point position)
        {
            if (element != null)
            {
                Canvas.SetLeft(element, position.X);
                Canvas.SetTop(element, position.Y);
            }
        }

        public bool Remove(IDeletable deletable)
        {
            throw new System.NotImplementedException();
        }

        public Point GetPosition(FrameworkElement element)
        {
            throw new System.NotImplementedException();
        }

        public IUIFunction Create(IFunction function)
        {
            //Todo: An actual relevant point, not just 200, 200
            return Create(function, new Point(200, 200));
        }

        public IUIFunction Create(IFunction function, Point location)
        {
            if (AssociatedProgram != null)
            {
                if (AssociatedProgram.Functions.Contains(function) || AssociatedProgram.AddFunction(function))
                {
                    var uiFunction = new UIFunction(function);
                    Add(uiFunction, location);

                    return uiFunction;
                }
            }

            return null;
        }

        public IUIFunction FindFunction(IFunction function)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(IUIConnection connection)
        {
            throw new System.NotImplementedException();
        }

        public Point GetBestSide(Rect rectangle, Point point)
        {
            throw new System.NotImplementedException();
        }

        public Point GetBestSide(FrameworkElement element, Point point)
        {
            throw new System.NotImplementedException();
        }

        public Point GetBestSide(FrameworkElement element, FrameworkElement otherElement)
        {
            throw new System.NotImplementedException();
        }

        public IUIConnection Create(IUIConnectionStart start, Point end)
        {
            throw new System.NotImplementedException();
        }

        public IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end)
        {
            throw new System.NotImplementedException();
        }

        public IUIConnection FindConnection(IUIConnectionStart start, IUIConnectionEnd end)
        {
            throw new System.NotImplementedException();
        }

        public List<IUIConnection> FindConnections(IUIConnectionStart start)
        {
            throw new System.NotImplementedException();
        }

        public List<IUIConnection> FindConnections(IUIConnectionEnd end)
        {
            throw new System.NotImplementedException();
        }

        public List<IUIConnection> FindConnections(List<IUIConnectionStart> starts)
        {
            throw new System.NotImplementedException();
        }

        public List<IUIConnection> FindConnections(List<IUIConnectionEnd> ends)
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

            VisualTreeHelper.HitTest(Canvas_Content, FilterHit, (result) => NewHit(result, hits), new PointHitTestParameters(point));

            hits.AddRange(new List<FrameworkElement>() { Canvas_Content, this });
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

        //Todo: Implement program loader
        protected void ChangeProgram(IProgram program)
        {
            Clear();

            if (_currentProgram != null)
            {
                //_ = _programLoader?.UnloadProgram(_currentProgram);
            }

            _currentProgram = program;

            if (_currentProgram != null)
            {
                //_ = _programLoader?.DisplayProgram(_currentProgram);
            }
        }

        private void Clear()
        {
            Canvas_Content.Children.Clear();
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

        private void Scroll_Content_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseMove(object sender, MouseEventArgs e)
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

        private void Scroll_Content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = _actionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = _actionHandler?.KeyDown(e.Key) ?? false;
        }

        private void Scroll_Content_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = _actionHandler?.KeyUp(e.Key) ?? false;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (ActualHeight / 2d));
        }
    }
}
