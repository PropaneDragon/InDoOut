using InDoOut_Core.Entities.Programs;
using InDoOut_Display.Actions;
using InDoOut_Display.Actions.Selecting;
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

        public ScreenConnections() : base()
        {
            InitializeComponent();

            SelectionManager = new ScreenConnectionsSelectionManager(this);
            ActionHandler = new ActionHandler(new ScreenConnectionsRestingAction(this));
            ProgramHandler = null; //Todo
            AssociatedProgram = new Program();
        }

        protected override bool ClearCurrentProgram()
        {
            //Todo

            return true;
        }

        private void Scroll_Content_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = ActionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = ActionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            bool handled;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                handled = ActionHandler?.MouseLeftMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                handled = ActionHandler?.MouseRightMove(e.GetPosition(sender as IInputElement)) ?? false;
            }
            else
            {
                handled = ActionHandler?.MouseNoMove(e.GetPosition(sender as IInputElement)) ?? false;
            }

            e.Handled = handled;
        }

        private void Scroll_Content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = ActionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = ActionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = ActionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement)) ?? false;
        }

        private void Scroll_Content_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = ActionHandler?.KeyDown(e.Key) ?? false;
        }

        private void Scroll_Content_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = ActionHandler?.KeyUp(e.Key) ?? false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Scroll_Content.ScrollToHorizontalOffset((Canvas_Content.ActualWidth / 2d) - (ActualWidth / 2d));
            Scroll_Content.ScrollToVerticalOffset((Canvas_Content.ActualHeight / 2d) - (ActualHeight / 2d));
        }
    }
}
