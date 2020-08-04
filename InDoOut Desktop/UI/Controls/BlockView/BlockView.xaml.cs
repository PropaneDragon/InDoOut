using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.Actions.Selecting;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Executable_Core.Programs;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Desktop.UI.Controls.BlockView
{
    public partial class BlockView : CommonProgramDisplay, IBlockView
    {
        public Point Offset { get => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset); set => SetViewOffset(value); }

        public override ISelectionManager<ISelectable> SelectionManager { get; protected set; }
        public override IActionHandler ActionHandler { get; protected set; }
        protected override IProgramHandler ProgramHandler { get; set; } = null;

        protected override Canvas ElementCanvas => Canvas_Content;

        protected override FrameworkElement HitTestElement => ElementCanvas;

        public override Size ViewSize => new Size(Scroll_Content.ActualWidth, Scroll_Content.ActualHeight);

        public override Point TopLeftViewCoordinate => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset);

        public override Point BottomRightViewCoordinate => new Point(TopLeftViewCoordinate.X + ViewSize.Width, TopLeftViewCoordinate.Y + ViewSize.Height);

        public override Point CentreViewCoordinate => new Point(TopLeftViewCoordinate.X + (ViewSize.Width / 2d), TopLeftViewCoordinate.Y + (ViewSize.Height / 2d));

        public BlockView() : base()
        {
            InitializeComponent();

            SelectionManager = new SelectionManager(this);
            ActionHandler = new ActionHandler(new BlockViewRestingAction(this));
            ProgramHandler = ProgramHolder.Instance;
            AssociatedProgram = ProgramHandler.NewProgram();

            BlockView_Overview.AssociatedBlockView = this;
        }

        public void MoveToCentre()
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (Scroll_Content.ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (Scroll_Content.ActualHeight / 2d));
        }

        protected override bool ClearCurrentProgram()
        {
            Canvas_Content.Children.Clear();

            return true;
        }

        protected override void ViewModeChanged(ProgramViewMode viewMode)
        {
        }

        protected override void ProgramChanged(IProgram program)
        {
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MoveToCentre();
        }

        private void Scroll_Content_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseLeftDown(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseLeftUp(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _ = ActionHandler?.MouseLeftMove(e.GetPosition(sender as ScrollViewer));
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                _ = ActionHandler?.MouseRightMove(e.GetPosition(sender as ScrollViewer));
            }
            else
            {
                _ = ActionHandler?.MouseNoMove(e.GetPosition(sender as ScrollViewer));
            }

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseRightDown(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseRightUp(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = ActionHandler?.MouseDoubleClick(e.GetPosition(sender as ScrollViewer));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            _ = ActionHandler?.KeyDown(e.Key);

            e.Handled = false;
        }

        private void Scroll_Content_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            _ = ActionHandler?.KeyUp(e.Key);

            e.Handled = false;
        }
    }
}
