using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.UI.Controls.CoreEntityRepresentation;
using InDoOut_Desktop.UI.Interfaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Desktop.UI.Controls.BlockView
{
    public partial class BlockView : UserControl, IBlockView
    {
        private ActionHandler _actionHandler = null;
        private IProgram _currentProgram = null;

        public IProgram AssociatedProgram { get => _currentProgram; set => ChangeProgram(value); }

        public Size TotalSize => new Size(Canvas_Content.ActualWidth, Canvas_Content.ActualHeight);

        public Size ViewSize => new Size(Scroll_Content.ActualWidth, Scroll_Content.ActualHeight);

        public Point TopLeftViewCoordinate => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset);

        public Point BottomRightViewCoordinate => new Point(TopLeftViewCoordinate.X + ViewSize.Width, TopLeftViewCoordinate.Y + ViewSize.Height);

        public Point CentreViewCoordinate => new Point(TopLeftViewCoordinate.X + (ViewSize.Width / 2d), TopLeftViewCoordinate.Y + (ViewSize.Height / 2d));

        public BlockView()
        {
            InitializeComponent();
            ChangeProgram(new Program());

            _actionHandler = new ActionHandler(new BlockViewRestingAction(Scroll_Content, this));
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

                Canvas.SetLeft(element, position.X);
                Canvas.SetTop(element, position.Y);
            }
        }

        public void Remove(FrameworkElement element)
        {
            if (element != null && Canvas_Content.Children.Contains(element))
            {
                Canvas_Content.Children.Remove(element);
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

        public IUIConnection FindConnection(IUIOutput output, IUIInput input)
        {
            throw new System.NotImplementedException();
        }

        public List<IUIConnection> FindConnections(IUIOutput output)
        {
            throw new System.NotImplementedException();
        }

        public List<IUIConnection> FindConnections(IUIInput input)
        {
            throw new System.NotImplementedException();
        }

        public IUIFunction FindFunction(IFunction function)
        {
            var children = Canvas_Content.Children;
            foreach (var child in children)
            {
                if (child != null && child is UIFunction uiFunction && uiFunction.AssociatedFunction == function)
                {
                    return uiFunction;
                }
            }

            return null;
        }

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (Scroll_Content.ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (Scroll_Content.ActualHeight / 2d));
        }

        private void Scroll_Content_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _actionHandler?.MouseLeftDown(e.GetPosition(sender as ScrollViewer));
        }

        private void Scroll_Content_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _actionHandler?.MouseLeftUp(e.GetPosition(sender as ScrollViewer));
        }

        private void Scroll_Content_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _actionHandler?.MouseLeftMove(e.GetPosition(sender as ScrollViewer));
        }
    }
}
