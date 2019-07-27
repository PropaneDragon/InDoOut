using InDoOut_Core.Instancing;
using System;
using System.Threading;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Threading
{
    internal class UIThread : Singleton<UIThread>
    {
        public Thread CurrentThread { get; private set; } = null;
        public Dispatcher CurrentDispatcher { get; private set; } = null;

        internal void SetCurrentThreadAsUIThread()
        {
            CurrentThread = Thread.CurrentThread;
            CurrentDispatcher = Dispatcher.CurrentDispatcher;
        }

        internal bool TryRunOnUI(Action action)
        {
            if (action != null && CurrentDispatcher != null)
            {
                try
                {
                    CurrentDispatcher.Invoke(action);

                    return true;
                }
                catch { }
            }

            return false;
        }
    }
}
