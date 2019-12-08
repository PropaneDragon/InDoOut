using InDoOut_Desktop.Loading;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Executable_Core.Loading;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Windows
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window, ISplashScreen
    {
        private bool _textUpdateNeeded = true;
        private ILoadingTask _taskToRun = null;
        private readonly DispatcherTimer _uiUpdateTimer = new DispatcherTimer(DispatcherPriority.Normal);

        internal TimeSpan HoldTime { get; set; } = TimeSpan.FromSeconds(5);

        public SplashWindow()
        {
            InitializeComponent();

            _uiUpdateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _uiUpdateTimer.Tick += UIUpdateTimer_Tick;
            _uiUpdateTimer.Start();
        }

        internal static async Task<bool> ShowForLoadingTaskAsync(Window owner, ILoadingTask task)
        {
            var splashWindow = new SplashWindow()
            {
                Owner = owner
            };

            splashWindow.Show();

            var startTime = DateTime.UtcNow;
            var result = await splashWindow.RunTaskAsync(task);
            var endTime = DateTime.UtcNow;
            var totalTime = endTime - startTime;

            if (totalTime < splashWindow.HoldTime)
            {
                _ = splashWindow.HoldTime - totalTime;

                //await Task.Delay(remainingTime);
            }

            splashWindow.Close();

            return result;
        }

        public async Task<bool> RunTaskAsync(ILoadingTask task)
        {
            if (task != null)
            {
                SetTaskToRun(task);

                return await Task.Run(() => task.RunAsync());
            }

            return false;
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

        private void TaskToRun_OnNameChanged(object sender, LoadingTaskEventArgs e)
        {
            _textUpdateNeeded = true;
        }

        private void UIUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_textUpdateNeeded)
            {
                _textUpdateNeeded = false;

                Text_LoadingMessage.Text = _taskToRun?.Name;
            }
        }
    }
}
