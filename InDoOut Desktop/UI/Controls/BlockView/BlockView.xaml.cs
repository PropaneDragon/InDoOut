using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Actions;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.BlockView
{
    public partial class BlockView : UserControl
    {
        private ActionHandler _actionHandler = null;

        public IProgram Program { get; set; } = null;

        public BlockView()
        {
            InitializeComponent();

            _actionHandler = new ActionHandler(new BlockViewRestingAction(Scroll_Content));
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
