using InDoOut_Desktop.Loading;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

[assembly: InternalsVisibleTo("InDoOut Desktop Tests")]
namespace InDoOut_Desktop.UI.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task FinishLoading()
        {

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
