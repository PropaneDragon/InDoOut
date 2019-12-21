using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Executable_Core.Loading;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.Splash
{
    public partial class SplashOverlay : UserControl, ISplashScreen
    {
        private bool _textUpdateNeeded = true;
        private ILoadingTask _taskToRun = null;
        private readonly DispatcherTimer _uiUpdateTimer = new DispatcherTimer(DispatcherPriority.Normal);

        public SplashOverlay()
        {
            InitializeComponent();

            _uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _uiUpdateTimer.Tick += UIUpdateTimer_Tick;
            _uiUpdateTimer.Start();

            UpdateVersion();
        }

        public async Task<bool> RunTaskAsync(ILoadingTask task)
        {
            var ran = false;

            Visibility = System.Windows.Visibility.Visible;

            if (task != null)
            {
                SetTaskToRun(task);

                ran = await Task.Run(() => task.RunAsync());
            }

            HideOverlay();

            return ran;
        }

        private void HideOverlay()
        {
            var fadeAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500));
            fadeAnimation.Completed += (sender, e) => Visibility = System.Windows.Visibility.Collapsed;

            BeginAnimation(OpacityProperty, fadeAnimation);
        }

        private void SetTaskToRun(ILoadingTask task)
        {
            if (_taskToRun != null)
            {
                _taskToRun.NameChanged -= TaskToRun_OnNameChanged;
            }

            _taskToRun = task;

            if (_taskToRun != null)
            {
                _taskToRun.NameChanged += TaskToRun_OnNameChanged;
            }
        }

        private void UpdateVersion()
        {
            var version = Assembly.GetEntryAssembly()?.GetName()?.Version;
            if (version != null)
            {
                Text_Version.Text = $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            }
        }

        private void TaskToRun_OnNameChanged(object sender, LoadingTaskEventArgs e)
        {
            _textUpdateNeeded = true;
        }

        private void UIUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_textUpdateNeeded)
            {
                _textUpdateNeeded = false;

                Text_Loading.Text = _taskToRun?.Name;
            }
        }
    }
}
