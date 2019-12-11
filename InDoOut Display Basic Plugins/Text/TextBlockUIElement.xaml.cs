using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_Display_Basic_Plugins.Text
{
    public partial class TextBlockUIElement : UserControl
    {
        private DispatcherTimer _updateTimer = null;

        public TextBlockElement Element { get; set; } = null;

        public TextBlockUIElement()
        {
            InitializeComponent();
        }

        private void UpdateFromElement()
        {
            if (Element.ShouldDisplayUpdate)
            {

            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateFromElement();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_updateTimer == null)
            {
                _updateTimer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = TimeSpan.FromMilliseconds(300),
                    IsEnabled = true
                };

                _updateTimer.Tick += UpdateTimer_Tick;
            }
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _updateTimer.Stop();
            _updateTimer.Tick -= UpdateTimer_Tick;
            _updateTimer = null;
        }
    }
}
