using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shell;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.Core
{
    public partial class TitleBar : UserControl
    {
        private DispatcherTimer _periodicUpdateTimer = null;
        private Window _attachedWindow = null;
        private string _lastTitle = null;

        public TitleBar()
        {
            InitializeComponent();
        }

        private void AttachToWindow()
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                _attachedWindow = window;
                _attachedWindow.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 50, 50, 50));
                _attachedWindow.BorderThickness = new Thickness(1);

                var chrome = new WindowChrome()
                {
                    GlassFrameThickness = new Thickness(1),
                    ResizeBorderThickness = new Thickness(4),
                    CornerRadius = new CornerRadius(0),
                    NonClientFrameEdges = NonClientFrameEdges.None,
                    CaptionHeight = 0,
                    UseAeroCaptionButtons = false
                };

                WindowChrome.SetWindowChrome(window, chrome);

                HwndSource source = HwndSource.FromHwnd(new WindowInteropHelper(_attachedWindow).Handle);
                source.AddHook(WindowHook);
            }
        }

        private void ToggleWindowState()
        {
            if (_attachedWindow != null)
            {
                var state = _attachedWindow.WindowState;

                _attachedWindow.WindowState = state == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        private IntPtr WindowHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x0084 /*WM_NCHITTEST*/)
            {
                // This prevents a crash in WindowChromeWorker._HandleNCHitTest
                try
                {
                    lParam.ToInt32();
                }
                catch (OverflowException)
                {
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            AttachToWindow();

            if (_periodicUpdateTimer == null)
            {
                _periodicUpdateTimer = new DispatcherTimer(DispatcherPriority.Normal)
                {
                    Interval = TimeSpan.FromMilliseconds(100)
                };

                _periodicUpdateTimer.Tick += PeriodicUpdateTimer_Tick;
                _periodicUpdateTimer.Start();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_periodicUpdateTimer != null)
            {
                _periodicUpdateTimer.Stop();
                _periodicUpdateTimer.Tick -= PeriodicUpdateTimer_Tick;
                _periodicUpdateTimer = null;
            }
        }

        private void PeriodicUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_attachedWindow != null)
            {
                var windowTitle = _attachedWindow.Title;

                if (_lastTitle == null || _lastTitle != windowTitle)
                {
                    _lastTitle = windowTitle;

                    var opacityAnimationOut = new DoubleAnimation(0, TimeSpan.FromMilliseconds(100));

                    opacityAnimationOut.Completed += (sender, e) =>
                    {
                        var opacityAnimationIn = new DoubleAnimation(1, TimeSpan.FromMilliseconds(100));

                        Text_Title.Text = _attachedWindow.Title;
                        Text_Title.BeginAnimation(OpacityProperty, opacityAnimationIn);
                    };

                    Text_Title.BeginAnimation(OpacityProperty, opacityAnimationOut);
                }
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (_attachedWindow != null)
            {
                _attachedWindow.Close();
            }
        }

        private void Button_Restore_Click(object sender, RoutedEventArgs e)
        {
            ToggleWindowState();
        }

        private void Button_Minimise_Click(object sender, RoutedEventArgs e)
        {
            if (_attachedWindow != null)
            {
                _attachedWindow.WindowState = WindowState.Minimized;
            }
        }

        private void UserControl_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ToggleWindowState();
        }

        private void Text_Title_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void Text_Title_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _attachedWindow != null)
            {
                if (_attachedWindow.WindowState == WindowState.Maximized)
                {
                    var mousePosition = _attachedWindow.PointToScreen(e.GetPosition(_attachedWindow));

                    _attachedWindow.Top = mousePosition.Y - (Text_Title.ActualHeight / 2d);
                    _attachedWindow.WindowState = WindowState.Normal;
                    _attachedWindow.Left = mousePosition.X - (_attachedWindow.ActualWidth / 2d);
                }

                try
                {
                    _attachedWindow.DragMove();
                }
                catch (OverflowException)
                {

                }
                catch { }
            }
        }
    }
}
