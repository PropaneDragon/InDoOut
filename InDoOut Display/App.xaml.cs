using InDoOut_Core.Logging;
using System;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_Display
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Log.Instance.Header("Desktop application started");

            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Instance.Header("Crashed!");
            Log.Instance.Error(e.Exception.Message);
            Log.Instance.Error(e.Exception.StackTrace);
        }
    }
}
