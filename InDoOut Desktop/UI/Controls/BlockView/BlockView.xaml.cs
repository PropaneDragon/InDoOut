﻿using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.Actions.Selecting;
using InDoOut_Desktop.Loading.BlockView;
using InDoOut_Desktop.Programs;
using InDoOut_Desktop.UI.Controls.CoreEntityRepresentation;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
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
        private readonly SelectionManager _selectionManager = null;
        private readonly BlockViewProgramLoader _programLoader = null;
        private readonly ActionHandler _actionHandler = null;
        private IProgram _currentProgram = null;
        private BlockViewMode _currentViewMode = BlockViewMode.IO;

        public IProgram AssociatedProgram { get => _currentProgram; set => ChangeProgram(value); }
        public BlockViewMode CurrentViewMode { get => _currentViewMode; set => ChangeViewMode(value); }
        public ISelectionManager<IBlockViewSelectable> SelectionManager => _selectionManager;

        public Size TotalSize => new Size(Canvas_Content.ActualWidth, Canvas_Content.ActualHeight);

        public Size ViewSize => new Size(Scroll_Content.ActualWidth, Scroll_Content.ActualHeight);

        public Point TopLeftViewCoordinate => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset);

        public Point BottomRightViewCoordinate => new Point(TopLeftViewCoordinate.X + ViewSize.Width, TopLeftViewCoordinate.Y + ViewSize.Height);

        public Point CentreViewCoordinate => new Point(TopLeftViewCoordinate.X + (ViewSize.Width / 2d), TopLeftViewCoordinate.Y + (ViewSize.Height / 2d));

        public Point Offset { get => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset); set => SetViewOffset(value); }

        public List<IUIFunction> UIFunctions => FindCanvasChild<IUIFunction>();

        public List<IUIConnection> UIConnections => FindCanvasChild<IUIConnection>();

        public List<FrameworkElement> Elements => FindCanvasChild<FrameworkElement>();

        public BlockView() : this(ProgramHolder.Instance.NewProgram())
        {
        }

        public BlockView(IProgram program)
        {
            InitializeComponent();

            BlockView_Overview.AssociatedBlockView = this;

            _selectionManager = new SelectionManager(this);
            _actionHandler = new ActionHandler(new BlockViewRestingAction(this));
            _programLoader = new BlockViewProgramLoader(this);

            ChangeProgram(program);
            ChangeViewMode(CurrentViewMode);
        }

        private void SetViewOffset(Point offset)
        {
            Scroll_Content.ScrollToHorizontalOffset(offset.X);
            Scroll_Content.ScrollToVerticalOffset(offset.Y);

            if (AssociatedProgram != null)
            {
                AssociatedProgram.Metadata["x"] = offset.X.ToString();
                AssociatedProgram.Metadata["y"] = offset.Y.ToString();
            }
        }

        public void Add(FrameworkElement element)
        {
            Add(element, CentreViewCoordinate);
        }

        public void Add(FrameworkElement element, Point position, int zIndex = 0)
        {
            if (element != null)
            {
                _ = Canvas_Content.Children.Add(element);

                Panel.SetZIndex(element, zIndex);

                SetPosition(element, position);
                ChangeViewMode(CurrentViewMode);
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

        public void MoveToCentre()
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (Scroll_Content.ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (Scroll_Content.ActualHeight / 2d));
        }

        public IUIFunction Create(IFunction function)
        {
            return Create(function, CentreViewCoordinate);
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

        public IUIConnection Create(IUIConnectionStart start, Point end)
        {
            if (start != null && start is FrameworkElement element)
            {
                var bestSidePoint = GetBestSide(element, end);
                var uiConnection = new UIConnection()
                {
                    Start = bestSidePoint,
                    End = end,
                    AssociatedStart = start
                };

                Add(uiConnection, new Point(0, 0), -999);

                return uiConnection;
            }

            return null;
        }

        public IUIConnection Create(IUIConnectionStart start, IUIConnectionEnd end)
        {
            if (start != null && end != null && end is FrameworkElement element)
            {
                var endPosition = GetPosition(element);
                var uiConnection = Create(start, endPosition);

                if (uiConnection != null)
                {
                    uiConnection.AssociatedEnd = end;

                    return uiConnection;
                }
            }

            return null;
        }

        public IUIConnection FindConnection(IUIConnectionStart start, IUIConnectionEnd end) => FindCanvasChild<IUIConnection>(uiConnection => uiConnection.AssociatedEnd == end && uiConnection.AssociatedStart == start).FirstOrDefault();
        public List<IUIConnection> FindConnections(IUIConnectionStart start) => FindConnections(new List<IUIConnectionStart>() { start });
        public List<IUIConnection> FindConnections(IUIConnectionEnd end) => FindConnections(new List<IUIConnectionEnd>() { end });
        public List<IUIConnection> FindConnections(List<IUIConnectionStart> starts) => FindCanvasChild<IUIConnection>(uiConnection => starts.Contains(uiConnection.AssociatedStart));
        public List<IUIConnection> FindConnections(List<IUIConnectionEnd> ends) => FindCanvasChild<IUIConnection>(uiConnection => ends.Contains(uiConnection.AssociatedEnd));
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

        public Point GetBestSide(Rect rectangle, Point point)
        {
            var topLeft = rectangle.TopLeft;
            var size = rectangle.Size;
            var centre = new Point(topLeft.X + (size.Width / 2d), topLeft.Y + (size.Height / 2d));

            return point.X < centre.X ? new Point(topLeft.X, centre.Y) : new Point(topLeft.X + size.Width, centre.Y);
        }

        public Point GetBestSide(FrameworkElement element, Point point)
        {
            return element != null ? GetBestSide(new Rect(GetPosition(element), new Size(element.ActualWidth, element.ActualHeight)), point) : point;
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
                _ = _programLoader?.UnloadProgram(_currentProgram);
            }

            _currentProgram = program;

            if (_currentProgram != null)
            {
                _ = _programLoader?.DisplayProgram(_currentProgram);
            }
        }

        private void ChangeViewMode(BlockViewMode viewMode)
        {
            _currentViewMode = viewMode;

            var functions = FindCanvasChild<IUIFunction>();
            var ioConnections = FindCanvasChild<IUIConnection>(connection => connection.AssociatedStart is IUIOutput);
            var variableConnections = FindCanvasChild<IUIConnection>(connection => connection.AssociatedStart is IUIResult);

            foreach (var function in functions)
            {
                function.DisplayMode = viewMode == BlockViewMode.IO ? UIFunctionDisplayMode.IO : UIFunctionDisplayMode.Variables;
            }

            foreach (var ioConnection in ioConnections)
            {
                ioConnection.Hidden = viewMode != BlockViewMode.IO;
            }

            foreach (var variableConnection in variableConnections)
            {
                variableConnection.Hidden = viewMode != BlockViewMode.Variables;
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
        }

        private void Scroll_Content_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseLeftDown(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseLeftUp(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _ = _actionHandler?.MouseLeftMove(e.GetPosition(sender as ScrollViewer));
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                _ = _actionHandler?.MouseRightMove(e.GetPosition(sender as ScrollViewer));
            }
            else
            {
                _ = _actionHandler?.MouseNoMove(e.GetPosition(sender as ScrollViewer));
            }

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseRightDown(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseRightUp(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseDoubleClick(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _ = _actionHandler?.KeyDown(e.Key);

            e.Handled = false;
        }

        private void Scroll_Content_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _ = _actionHandler?.KeyUp(e.Key);

            e.Handled = false;
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            MoveToCentre();
        }
    }
}
