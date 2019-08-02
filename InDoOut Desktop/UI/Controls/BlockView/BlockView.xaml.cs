using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.UI.Controls.CoreEntityRepresentation;
using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InDoOut_Desktop.UI.Controls.BlockView
{
    public partial class BlockView : UserControl, IBlockView, IScrollable
    {
        private ActionHandler _actionHandler = null;
        private IProgram _currentProgram = null;

        public IProgram AssociatedProgram { get => _currentProgram; set => ChangeProgram(value); }

        public Size TotalSize => new Size(Canvas_Content.ActualWidth, Canvas_Content.ActualHeight);

        public Size ViewSize => new Size(Scroll_Content.ActualWidth, Scroll_Content.ActualHeight);

        public Point TopLeftViewCoordinate => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset);

        public Point BottomRightViewCoordinate => new Point(TopLeftViewCoordinate.X + ViewSize.Width, TopLeftViewCoordinate.Y + ViewSize.Height);

        public Point CentreViewCoordinate => new Point(TopLeftViewCoordinate.X + (ViewSize.Width / 2d), TopLeftViewCoordinate.Y + (ViewSize.Height / 2d));

        public Point Offset { get => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset); set { Scroll_Content.ScrollToHorizontalOffset(value.X); Scroll_Content.ScrollToVerticalOffset(value.Y); } }

        public BlockView()
        {
            InitializeComponent();
            ChangeProgram(new Program());

            _actionHandler = new ActionHandler(new BlockViewRestingAction(this));
        }

        public void Add(FrameworkElement element)
        {
            Add(element, CentreViewCoordinate);
        }

        public void Add(FrameworkElement element, Point position)
        {
            if (element != null)
            {
                Canvas_Content.Children.Add(element);

                SetPosition(element, position);
            }
        }

        public void Remove(FrameworkElement element)
        {
            if (element != null && Canvas_Content.Children.Contains(element))
            {
                Canvas_Content.Children.Remove(element);
            }
        }

        public void SetPosition(FrameworkElement element, Point position)
        {
            if (element != null)
            {
                Canvas.SetLeft(element, position.X);
                Canvas.SetTop(element, position.Y);
            }
        }

        public IUIFunction Create(IFunction function)
        {
            return Create(function, CentreViewCoordinate);
        }

        public IUIFunction Create(IFunction function, Point location)
        {
            if (AssociatedProgram != null)
            {
                if (AssociatedProgram.AddFunction(function))
                {
                    var uiFunction = new UIFunction(function);

                    Add(uiFunction, location);

                    return uiFunction;
                }
            }

            return null;
        }

        public IUIConnection Create(IUIOutput start, Point end)
        {
            if (start != null && start is FrameworkElement element)
            {
                var bestSidePoint = GetBestSide(element, end);
                var uiConnection = new UIConnection()
                {
                    Start = bestSidePoint,
                    End = end,
                    AssociatedOutput = start
                };

                Add(uiConnection, new Point(0, 0));

                return uiConnection;
            }

            return null;
        }

        public IUIConnection Create(IUIOutput start, IUIInput end)
        {
            if (start != null && end != null && end is FrameworkElement element)
            {
                var endPosition = GetPosition(element);
                var uiConnection = Create(start, endPosition);   
                
                if (uiConnection != null)
                {
                    uiConnection.AssociatedInput = end;

                    return uiConnection;
                }
            }

            return null;
        }

        public IUIConnection FindConnection(IUIOutput output, IUIInput input) => FindCanvasChild<IUIConnection>(uiConnection => uiConnection.AssociatedInput == input && uiConnection.AssociatedOutput == output).FirstOrDefault();
        public List<IUIConnection> FindConnections(IUIOutput output) => FindConnections(new List<IUIOutput>() { output });
        public List<IUIConnection> FindConnections(IUIInput input) => FindConnections(new List<IUIInput>() { input });
        public List<IUIConnection> FindConnections(List<IUIOutput> outputs) => FindCanvasChild<IUIConnection>(uiConnection => outputs.Contains(uiConnection.AssociatedOutput));
        public List<IUIConnection> FindConnections(List<IUIInput> inputs) => FindCanvasChild<IUIConnection>(uiConnection => inputs.Contains(uiConnection.AssociatedInput));
        public IUIFunction FindFunction(IFunction function) => FindCanvasChild<IUIFunction>(uiFunction => uiFunction.AssociatedFunction == function).FirstOrDefault();

        public void Remove(IUIConnection output)
        {
            if (output is FrameworkElement element)
            {
                Remove(element);
            }
        }

        public Point GetMousePosition()
        {
            var mousePosition = Mouse.GetPosition(Canvas_Content);
            return mousePosition;
        }

        public Point GetPosition(FrameworkElement element)
        {
            if (element != null)
            {
                var relativePoint = element.TranslatePoint(new Point(0, 0), Canvas_Content);
                return relativePoint;
            }

            return new Point(0, 0);
        }

        public Point GetBestSide(FrameworkElement element, Point point)
        {
            if (element != null)
            {
                var topLeft = GetPosition(element);
                var size = new Size(element.ActualWidth, element.ActualHeight);
                var centre = new Point(topLeft.X + (size.Width / 2d), topLeft.Y + (size.Height / 2d));

                if (point.X < centre.X)
                {
                    return new Point(topLeft.X, centre.Y);
                }

                return new Point(topLeft.X + size.Width, centre.Y);
            }

            return point;
        }

        public Point GetBestSide(FrameworkElement element, FrameworkElement otherElement)
        {
            if (element != null && otherElement != null)
            {
                var otherElementPosition = GetPosition(otherElement);

                return GetBestSide(element, otherElementPosition);
            }

            return new Point();
        }

        public List<FrameworkElement> GetElementsUnderMouse()
        {
            return GetElementsAtPoint(GetMousePosition());
        }

        public List<FrameworkElement> GetElementsAtPoint(Point point)
        {
            var hits = new List<FrameworkElement>() { Canvas_Content, this };

            VisualTreeHelper.HitTest(Canvas_Content, FilterHit, (result) => NewHit(result, hits), new PointHitTestParameters(point));

            return hits;
        }

        private HitTestFilterBehavior FilterHit(DependencyObject potentialHitTestTarget)
        {
            return HitTestFilterBehavior.Continue;
        }

        private HitTestResultBehavior NewHit(HitTestResult result, List<FrameworkElement> hits)
        {
            if (result.VisualHit != null && result.VisualHit is FrameworkElement element)
            {
                hits.Add(element);
            }

            return HitTestResultBehavior.Continue;
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

        protected void ChangeProgram(IProgram program)
        {
            Clear();

            if (_currentProgram != null)
            {
                //Todo: Add teardown from the previous program.
            }

            _currentProgram = program;

            if (_currentProgram != null)
            {
                foreach (var function in _currentProgram.Functions)
                {
                    var visualFunction = Create(function);
                }
            }
        }

        private void Clear()
        {
            Canvas_Content.Children.Clear();
        }

        private List<T> FindCanvasChild<T>() where T : class
        {
            return FindCanvasChild<T>(T => true);
        }

        private List<T> FindCanvasChild<T>(Func<T, bool> matchFunction) where T : class
        {
            var validChildren = new List<T>();
            var children = Canvas_Content.Children;

            foreach (var child in children)
            {
                if (child != null && child is T type && (matchFunction?.Invoke(type) ?? true))
                {
                    validChildren.Add(type);
                }
            }

            return validChildren;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (Scroll_Content.ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (Scroll_Content.ActualHeight / 2d));
        }

        private void Scroll_Content_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _actionHandler?.MouseLeftDown(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _actionHandler?.MouseLeftUp(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_MouseMove(object sender, MouseEventArgs e)
        {
            _actionHandler?.MouseLeftMove(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }
    }
}
