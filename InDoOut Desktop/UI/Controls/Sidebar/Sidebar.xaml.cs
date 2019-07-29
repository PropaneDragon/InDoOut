using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InDoOut_Desktop.UI.Controls.Sidebar
{
    public partial class Sidebar : UserControl
    {
        private bool _collapsed = false;
        private TimeSpan _animationTime = TimeSpan.FromMilliseconds(500);

        public bool Collapsed { get => _collapsed; set { if (value) Collapse(); else Expand(); } }
        public ItemList ItemList => ItemList_Functions;

        public Sidebar()
        {
            InitializeComponent();
        }

        public void Collapse()
        {
            if (!_collapsed)
            {
                var offset = -(ActualWidth - ColumnDefinition_Extended.Width.Value);
                var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                var sidebarAnimation = new ThicknessAnimation(new Thickness(offset, 0, 0, 0), _animationTime) { EasingFunction = easingFunction };
                var opacityAnimation = new DoubleAnimation(0, _animationTime) { EasingFunction = easingFunction };

                BeginAnimation(MarginProperty, sidebarAnimation);
                Grid_CollapsibleContent.BeginAnimation(OpacityProperty, opacityAnimation);

                sidebarAnimation.Completed += (sender, e) => Grid_CollapsibleContent.Visibility = Visibility.Hidden;                
            }

            _collapsed = true;
        }

        public void Expand()
        {
            if (_collapsed)
            {
                Grid_CollapsibleContent.Visibility = Visibility.Visible;

                var easingFunction = new ExponentialEase() { EasingMode = EasingMode.EaseInOut };
                var sidebarAnimation = new ThicknessAnimation(new Thickness(0, 0, 0, 0), _animationTime) { EasingFunction = easingFunction };
                var opacityAnimation = new DoubleAnimation(1, _animationTime) { EasingFunction = easingFunction }; ;

                BeginAnimation(MarginProperty, sidebarAnimation);
                Grid_CollapsibleContent.BeginAnimation(OpacityProperty, opacityAnimation);
            }

            _collapsed = false;
        }

        private void Button_Collapse_Click(object sender, RoutedEventArgs e)
        {
            Collapsed = !Collapsed;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Collapsed = true;
        }

        private void SearchBar_SearchRequested(object sender, Search.SearchArgs e)
        {
            ItemList_Functions.Filter(e.Query);
        }
    }
}
