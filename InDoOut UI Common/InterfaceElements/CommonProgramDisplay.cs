using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Executable_Core.Programs;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.Creation;
using InDoOut_UI_Common.Removal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InDoOut_UI_Common.InterfaceElements
{
    public abstract class CommonProgramDisplay : UserControl, ICommonProgramDisplay
    {
        private IProgram _currentProgram = null;
        private ProgramViewMode _currentViewMode = ProgramViewMode.IO;

        protected ICommonProgramLoader CommonProgramLoader { get; set; } = null;

        public IProgram AssociatedProgram { get => _currentProgram; set => ChangeProgram(value); }
        public IFunctionCreator FunctionCreator { get; protected set; }
        public IConnectionCreator ConnectionCreator { get; protected set; }
        public IDeletableRemover DeletableRemover { get; protected set; }
        public ProgramViewMode CurrentViewMode { get => _currentViewMode; set => ChangeViewMode(value); }
        public List<IUIFunction> UIFunctions => FindCanvasChild<IUIFunction>();
        public List<IUIConnection> UIConnections => FindCanvasChild<IUIConnection>();
        public List<FrameworkElement> Elements => FindCanvasChild<FrameworkElement>();
        public Size TotalSize => new(ElementCanvas.ActualWidth, ElementCanvas.ActualHeight);

        public abstract ISelectionManager<ISelectable> SelectionManager { get; protected set; }
        public abstract IActionHandler ActionHandler { get; protected set; }
        protected abstract IProgramHandler ProgramHandler { get; set; }
        protected abstract Canvas ElementCanvas { get; }
        protected abstract FrameworkElement HitTestElement { get; }

        public abstract Size ViewSize { get; }
        public abstract Point TopLeftViewCoordinate { get; }
        public abstract Point BottomRightViewCoordinate { get; }
        public abstract Point CentreViewCoordinate { get; }

        public CommonProgramDisplay() : base()
        {
            CommonProgramLoader = new CommonProgramLoader(this);
            FunctionCreator = new BasicFunctionCreator(this);
            ConnectionCreator = new BasicConnectionCreator(this);
            DeletableRemover = new BasicDeletableRemover(this);

            ChangeViewMode(CurrentViewMode);
        }

        public void Add(FrameworkElement element) => Add(element, CentreViewCoordinate);

        public void Add(FrameworkElement element, Point position, int zIndex = 0)
        {
            if (element != null)
            {
                _ = ElementCanvas.Children.Add(element);

                Panel.SetZIndex(element, zIndex);

                SetPosition(element, position);
                ChangeViewMode(CurrentViewMode);
            }
        }

        public void Remove(FrameworkElement element)
        {
            if (element != null && ElementCanvas.Children.Contains(element))
            {
                ElementCanvas.Children.Remove(element);
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

        public List<FrameworkElement> GetElementsUnderMouse() => GetElementsAtPoint(GetMousePosition());

        public List<FrameworkElement> GetElementsAtPoint(Point point)
        {
            var hits = new List<FrameworkElement>() { HitTestElement, this };

            VisualTreeHelper.HitTest(HitTestElement, FilterHit, (result) => NewHit(result, hits), new PointHitTestParameters(point));

            return hits;
        }

        public Point GetBestSide(FrameworkElement element, Point point) => element != null ? GetBestSide(new Rect(GetPosition(element), new Size(element.ActualWidth, element.ActualHeight)), point) : point;
        public Point GetBestSide(FrameworkElement element, FrameworkElement otherElement)
        {
            if (element != null && otherElement != null)
            {
                var otherElementPosition = GetPosition(otherElement);

                return GetBestSide(element, otherElementPosition);
            }

            return new Point();
        }

        public Point GetPosition(FrameworkElement element)
        {
            if (element != null)
            {
                var relativePoint = element.TranslatePoint(new Point(0, 0), ElementCanvas);
                return relativePoint;
            }

            return new Point(0, 0);
        }

        public Point GetMousePosition()
        {
            var mousePosition = Mouse.GetPosition(ElementCanvas);
            return mousePosition;
        }

        public Point GetBestSide(Rect rectangle, Point point)
        {
            var topLeft = rectangle.TopLeft;
            var size = rectangle.Size;
            var centre = new Point(topLeft.X + (size.Width / 2d), topLeft.Y + (size.Height / 2d));

            return point.X < centre.X ? new Point(topLeft.X, centre.Y) : new Point(topLeft.X + size.Width, centre.Y);
        }

        public Size GetSize(FrameworkElement element) => element != null ? element.RenderSize : new Size(0, 0);
        public IUIConnection FindConnection(IUIConnectionStart start, IUIConnectionEnd end) => FindCanvasChild<IUIConnection>(uiConnection => uiConnection.AssociatedEnd == end && uiConnection.AssociatedStart == start).FirstOrDefault();
        public List<IUIConnection> FindConnections(IUIConnectionStart start) => FindConnections(new List<IUIConnectionStart>() { start });
        public List<IUIConnection> FindConnections(IUIConnectionEnd end) => FindConnections(new List<IUIConnectionEnd>() { end });
        public List<IUIConnection> FindConnections(List<IUIConnectionStart> starts) => FindCanvasChild<IUIConnection>(uiConnection => starts.Contains(uiConnection.AssociatedStart));
        public List<IUIConnection> FindConnections(List<IUIConnectionEnd> ends) => FindCanvasChild<IUIConnection>(uiConnection => ends.Contains(uiConnection.AssociatedEnd));
        public IUIFunction FindFunction(IFunction function) => FindCanvasChild<IUIFunction>(uiFunction => uiFunction.AssociatedFunction == function).FirstOrDefault();

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
            if (_currentProgram != null && program == _currentProgram)
            {
                UpdateProgram(program);
            }
            else
            {
                if (ClearCurrentProgram())
                {
                    if (_currentProgram != null)
                    {
                        _ = CommonProgramLoader?.UnloadProgram(_currentProgram);
                    }

                    _currentProgram = program;

                    ProgramChanged(program);
                    UpdateProgram(program);
                }
            }
        }

        protected void ChangeViewMode(ProgramViewMode viewMode)
        {
            _currentViewMode = viewMode;

            var functions = FindCanvasChild<IUIFunction>();
            var ioConnections = FindCanvasChild<IUIConnection>(connection => connection.AssociatedStart is IUIOutput);
            var variableConnections = FindCanvasChild<IUIConnection>(connection => connection.AssociatedStart is IUIResult);

            foreach (var function in functions)
            {
                function.DisplayMode = viewMode == ProgramViewMode.IO ? UIFunctionDisplayMode.IO : UIFunctionDisplayMode.Variables;
            }

            foreach (var ioConnection in ioConnections)
            {
                ioConnection.Hidden = viewMode != ProgramViewMode.IO;
            }

            foreach (var variableConnection in variableConnections)
            {
                variableConnection.Hidden = viewMode != ProgramViewMode.Variables;
            }

            ViewModeChanged(viewMode);
        }

        protected abstract void ViewModeChanged(ProgramViewMode viewMode);
        protected abstract void ProgramChanged(IProgram program);
        protected abstract bool ClearCurrentProgram();

        private void UpdateProgram(IProgram program)
        {
            if (ClearCurrentProgram())
            {
                if (program != null)
                {
                    _ = CommonProgramLoader?.DisplayProgram(program);
                }
            }
        }

        private List<T> FindCanvasChild<T>() where T : class => FindCanvasChild<T>(T => true);
        private List<T> FindCanvasChild<T>(Func<T, bool> matchFunction) where T : class
        {
            var validChildren = new List<T>();
            var children = ElementCanvas?.Children;

            if (children != null)
            {
                foreach (var child in children)
                {
                    if (child != null && child is T type && (matchFunction?.Invoke(type) ?? true))
                    {
                        validChildren.Add(type);
                    }
                }
            }

            return validChildren;
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
    }
}
