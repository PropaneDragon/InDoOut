using InDoOut_Desktop.Loading;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Desktop.UI.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace InDoOut_Desktop.UI.Windows
{
    public partial class MainWindow : Window
    {
        private static readonly bool OLD_SPLASH = false;

        public MainWindow()
        {
            InitializeComponent();

            UIThread.Instance.SetCurrentThreadAsUIThread();
        }

        private async Task FinishLoading()
        {
            var sidebar = Sidebar_Main;
            var blockView = BlockView_Main;
            var overview = BlockView_Overview;

            if (blockView != null)
            {
                if (sidebar != null)
                {
                    sidebar.BlockView = blockView;
                }

                if (overview != null)
                {
                    overview.AssociatedBlockView = blockView;
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var splash = OLD_SPLASH ? new SplashWindow() { Owner = this } : (ISplashScreen)Splash_Overlay;
            if (splash != null)
            {
                if (await splash.RunTaskAsync(new MainWindowLoadingTask()))
                {
                    await FinishLoading();
                    return;
                }
            }

            Close();
        }
    }
}
