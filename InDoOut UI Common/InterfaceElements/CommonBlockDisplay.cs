using InDoOut_Core.Entities.Programs;
using InDoOut_Executable_Core.Programs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_UI_Common.InterfaceElements
{
    public abstract class CommonBlockDisplay : CommonProgramDisplay, ICommonBlockDisplay
    {
        protected override IProgramHandler ProgramHandler { get; set; } = null;

        protected override FrameworkElement HitTestElement => ElementCanvas;

        protected abstract ScrollViewer ScrollViewer { get; }

        public override Size ViewSize => new(ScrollViewer.ActualWidth, ScrollViewer.ActualHeight);
        public override Point TopLeftViewCoordinate => new(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset);
        public override Point BottomRightViewCoordinate => new(TopLeftViewCoordinate.X + ViewSize.Width, TopLeftViewCoordinate.Y + ViewSize.Height);
        public override Point CentreViewCoordinate => new(TopLeftViewCoordinate.X + (ViewSize.Width / 2d), TopLeftViewCoordinate.Y + (ViewSize.Height / 2d));

        public Point Offset { get => new(ScrollViewer.HorizontalOffset, ScrollViewer.VerticalOffset); set => SetViewOffset(value); }

        public CommonBlockDisplay() : base()
        {
            ProgramHandler = ProgramHolder.Instance;

            AttachUserControlEvents();
        }

        public void MoveToCentre()
        {
            if (IsLoaded && ScrollViewer.ActualWidth > 0 && ElementCanvas.ActualWidth > 0)
            {
                ScrollViewer.ScrollToHorizontalOffset((ElementCanvas.ActualWidth / 2d) - (ScrollViewer.ActualWidth / 2d));
                ScrollViewer.ScrollToVerticalOffset((ElementCanvas.ActualHeight / 2d) - (ScrollViewer.ActualHeight / 2d));
            }
        }

        protected override bool ClearCurrentProgram()
        {
            ElementCanvas.Children.Clear();

            return true;
        }

        protected override void ViewModeChanged(ProgramViewMode viewMode)
        {
        }

        protected override void ProgramChanged(IProgram program)
        {
        }

        private void AttachScrollViewerEvents()
        {
            if (ScrollViewer != null)
            {
                ScrollViewer.Loaded += ScrollViewer_Loaded;
                ScrollViewer.PreviewMouseLeftButtonDown += ScrollViewer_MouseLeftButtonDown;
                ScrollViewer.PreviewMouseLeftButtonUp += ScrollViewer_MouseLeftButtonUp;
                ScrollViewer.PreviewMouseRightButtonDown += ScrollViewer_PreviewMouseRightButtonDown;
                ScrollViewer.PreviewMouseRightButtonUp += ScrollViewer_PreviewMouseRightButtonUp;
                ScrollViewer.PreviewMouseMove += ScrollViewer_MouseMove;
                ScrollViewer.PreviewMouseDoubleClick += ScrollViewer_PreviewMouseDoubleClick;
                ScrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
                ScrollViewer.PreviewKeyDown += ScrollViewer_PreviewKeyDown;
                ScrollViewer.PreviewKeyUp += ScrollViewer_PreviewKeyUp;
                ScrollViewer.PreviewDragOver += ScrollViewer_PreviewDragOver;
                ScrollViewer.PreviewDragEnter += ScrollViewer_PreviewDragEnter;
                ScrollViewer.PreviewDragLeave += ScrollViewer_PreviewDragLeave;
                ScrollViewer.Drop += ScrollViewer_Drop;
            }
        }

        private void AttachCanvasEvents()
        {
            if (ElementCanvas != null)
            {
                ElementCanvas.Loaded += ElementCanvas_Loaded;
            }
        }

        private void AttachUserControlEvents() => Loaded += CommonBlockDisplay_Loaded;

        private void SetViewOffset(Point offset)
        {
            ScrollViewer.ScrollToHorizontalOffset(offset.X);
            ScrollViewer.ScrollToVerticalOffset(offset.Y);

            if (AssociatedProgram != null)
            {
                AssociatedProgram.Metadata["x"] = offset.X.ToString();
                AssociatedProgram.Metadata["y"] = offset.Y.ToString();
            }
        }

        private void CommonBlockDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            AttachScrollViewerEvents();
            AttachCanvasEvents();
            MoveToCentre();
        }

        private void ElementCanvas_Loaded(object sender, RoutedEventArgs e) => MoveToCentre();

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e) => MoveToCentre();

        private void ScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseLeftDown(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseLeftUp(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0045:Convert to conditional expression", Justification = "It looks awful")]
        private void ScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _ = ActionHandler?.MouseLeftMove(e.GetPosition(sender as ScrollViewer));
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                _ = ActionHandler?.MouseRightMove(e.GetPosition(sender as ScrollViewer));
            }
            else
            {
                _ = ActionHandler?.MouseNoMove(e.GetPosition(sender as ScrollViewer));
            }

            e.Handled = false;
        }

        private void ScrollViewer_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseRightDown(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void ScrollViewer_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseRightUp(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void ScrollViewer_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseDoubleClick(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            _ = ActionHandler?.MouseWheel(e.Delta);

            e.Handled = false;
        }

        private void ScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _ = ActionHandler?.KeyDown(e.Key);

            e.Handled = false;
        }

        private void ScrollViewer_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _ = ActionHandler?.KeyUp(e.Key);

            e.Handled = false;
        }

        private void ScrollViewer_PreviewDragEnter(object sender, DragEventArgs e)
        {
            _ = ActionHandler?.DragEnter(e.GetPosition(ElementCanvas), e.Data);

            e.Handled = false;
        }

        private void ScrollViewer_PreviewDragOver(object sender, DragEventArgs e)
        {
            _ = ActionHandler?.DragOver(e.GetPosition(ElementCanvas), e.Data);

            e.Handled = false;
        }

        private void ScrollViewer_PreviewDragLeave(object sender, DragEventArgs e)
        {
            _ = ActionHandler?.DragLeave(e.GetPosition(ElementCanvas), e.Data);

            e.Handled = false;
        }

        private void ScrollViewer_Drop(object sender, DragEventArgs e)
        {
            _ = ActionHandler?.Drop(e.GetPosition(ElementCanvas), e.Data);

            e.Handled = false;
        }
    }
}
