using InDoOut_Display.Actions;
using InDoOut_UI_Common.Actions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InDoOut_Display.UI.Controls.Screens
{
    public partial class ScreenOverview : UserControl
    {
        private readonly ActionHandler _actionHandler = null;

        public ScreenOverview()
        {
            InitializeComponent();

            _actionHandler = new ActionHandler(new ScreenItemRestingAction(ScreenItem_Overview));
        }

        private void Scroll_Content_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseLeftDown(e.GetPosition(sender as IInputElement));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseLeftUp(e.GetPosition(sender as IInputElement));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _ = _actionHandler?.MouseLeftMove(e.GetPosition(sender as IInputElement));
            }
#pragma warning disable IDE0045 // Convert to conditional expression
            else if (e.RightButton == MouseButtonState.Pressed)
#pragma warning restore IDE0045 // Convert to conditional expression
            {
                _ = _actionHandler?.MouseRightMove(e.GetPosition(sender as IInputElement));
            }
            else
            {
                _ = _actionHandler?.MouseNoMove(e.GetPosition(sender as IInputElement));
            }

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseRightDown(e.GetPosition(sender as IInputElement));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseRightUp(e.GetPosition(sender as IInputElement));

            e.Handled = false;
        }

        private void Scroll_Content_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _ = _actionHandler?.MouseDoubleClick(e.GetPosition(sender as IInputElement));

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
    }
}
