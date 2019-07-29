using InDoOut_Desktop.Loading;
using InDoOut_Desktop.UI.Threading;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

[assembly: InternalsVisibleTo("InDoOut Desktop Tests")]
namespace InDoOut_Desktop.UI.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            UIThread.Instance.SetCurrentThreadAsUIThread();
        }

        private async Task FinishLoading()
        {
            var itemList = Sidebar_Main.ItemList;
            var blockView = BlockView_Main;

            if (itemList != null && blockView != null)
            {
                itemList.FunctionView = blockView;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (await SplashWindow.ShowForLoadingTaskAsync(this, new MainWindowLoadingTask()))
            {
                await FinishLoading();
            }
            else
            {
                Close();
            }
        }
    }
}
