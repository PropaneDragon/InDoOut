using System;

namespace InDoOut_Desktop.Loading
{
    public abstract class LoadingTaskEventArgs : EventArgs
    {
        public ILoadingTask BaseTask { get; protected set; }
    }

    public class LoadingTaskEventArgs<T> : LoadingTaskEventArgs where T : class, ILoadingTask
    {
        public T Task => BaseTask as T;

        public LoadingTaskEventArgs(T task)
        {
            BaseTask = task;
        }
    }
}
