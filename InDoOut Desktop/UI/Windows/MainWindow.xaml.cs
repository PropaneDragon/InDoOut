using InDoOut_Desktop.Loading;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Desktop.UI.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

[assembly: InternalsVisibleTo("InDoOut Desktop Tests")]
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

            if (sidebar != null && blockView != null)
            {
                sidebar.BlockView = blockView;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ISplashScreen splash = null;

            if (OLD_SPLASH)
            {
                splash = new SplashWindow() { Owner = this };
            }
            else
            {
                splash = Splash_Overlay;
            }

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
