using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Actions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class ScreenOverview : UserControl, IScreenOverview
    {
        private readonly ActionHandler _actionHandler = null;

        public IScreenConnections CurrentConnectionsScreen => ScreenConnections_Main;

        public ScreenOverview()
        {
            InitializeComponent();
        }

        private void Scroll_Content_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => e.Handled = _actionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement)) ?? false;

        private void Scroll_Content_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) => e.Handled = _actionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement)) ?? false;

        private void Scroll_Content_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            bool handled;

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

        private void Scroll_Content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e) => e.Handled = _actionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement)) ?? false;

        private void Scroll_Content_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e) => e.Handled = _actionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement)) ?? false;

        private void Scroll_Content_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) => e.Handled = _actionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement)) ?? false;

        private void Scroll_Content_PreviewKeyDown(object sender, KeyEventArgs e) => e.Handled = _actionHandler?.KeyDown(e.Key) ?? false;

        private void Scroll_Content_PreviewKeyUp(object sender, KeyEventArgs e) => e.Handled = _actionHandler?.KeyUp(e.Key) ?? false;
    }
}
