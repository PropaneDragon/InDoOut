using InDoOut_Core.Logging;
using System.Runtime.CompilerServices;
using System.Windows;

[assembly: InternalsVisibleTo("InDoOut Desktop Tests")]
namespace InDoOut_Desktop
{
    public partial class App : Application
    {
        public App()
        {
            Log.Instance.Header("Desktop application started");

            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {

        }
    }
}
