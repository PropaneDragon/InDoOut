using System;

namespace InDoOut_Desktop.Loading
{
    internal abstract class LoadingTaskEventArgs : EventArgs
    {
        public ILoadingTask BaseTask { get; protected set; }
    }

    internal class LoadingTaskEventArgs<T> : LoadingTaskEventArgs where T : class, ILoadingTask
    {
        public T Task => BaseTask as T;

        public LoadingTaskEventArgs(T task)
        {
            BaseTask = task;
        }
    }
}
