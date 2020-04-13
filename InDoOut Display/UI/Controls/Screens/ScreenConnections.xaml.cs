using InDoOut_Core.Entities.Programs;
using InDoOut_Display.Actions;
using InDoOut_Display.Actions.Selecting;
using InDoOut_Display.Creation;
using InDoOut_Display_Core.Screens;
using InDoOut_Executable_Core.Programs;
using InDoOut_UI_Common.Actions;
using InDoOut_UI_Common.Actions.Selecting;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class ScreenConnections : CommonProgramDisplay, IScreenConnections
    {
        public IScreen CurrentScreen => ScreenItem_Overview;

        public override Size ViewSize => new Size(Scroll_Content.ActualWidth, Scroll_Content.ActualHeight);

        public override Point TopLeftViewCoordinate => new Point(Scroll_Content.HorizontalOffset, Scroll_Content.VerticalOffset);

        public override Point BottomRightViewCoordinate => new Point(TopLeftViewCoordinate.X + ViewSize.Width, TopLeftViewCoordinate.Y + ViewSize.Height);

        public override Point CentreViewCoordinate => new Point(TopLeftViewCoordinate.X + (ViewSize.Width / 2d), TopLeftViewCoordinate.Y + (ViewSize.Height / 2d));

        public override ISelectionManager<ISelectable> SelectionManager { get; protected set; }

        public override IActionHandler ActionHandler { get; protected set; }

        protected override IProgramHandler ProgramHandler { get; set; } = null;

        protected override Canvas ElementCanvas => Canvas_Content;

        protected override FrameworkElement HitTestElement => Grid_CombinedContent;

        public ScreenConnections() : base()
        {
            InitializeComponent();

            FunctionFactory = new ExtendedFunctionCreator(this);
            SelectionManager = new ScreenConnectionsSelectionManager(this);
            ActionHandler = new ActionHandler(new ScreenConnectionsRestingAction(this));
            ProgramHandler = null; //Todo
        }

        protected override bool ClearCurrentProgram()
        {
            Canvas_Content.Children.Clear();

            return true;
        }

        protected override void ViewModeChanged(ProgramViewMode viewMode)
        {
            if (CurrentScreen != null)
            {
                CurrentScreen.CurrentViewMode = viewMode;
            }
        }

        protected override void ProgramChanged(IProgram program)
        {
            if (CurrentScreen != null)
            {
                CurrentScreen.AssociatedProgram = program;
            }
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (ActualHeight / 2d));
        }
    }
}
