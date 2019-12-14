using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InDoOut_UI_Common.Controls.Popup
{
    public partial class OverlayPopup : UserControl
    {
        public OverlayPopup()
        {
            InitializeComponent();
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100)) { EasingFunction = easingFunction };

            BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        public void Hide()
        {
            var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100)) { EasingFunction = easingFunction };

            fadeOutAnimation.Completed += (sender, e) => Visibility = Visibility.Collapsed;

            BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
        }

        private void Border_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Hide();

            e.Handled = Visibility == Visibility.Visible;
        }
    }
}
