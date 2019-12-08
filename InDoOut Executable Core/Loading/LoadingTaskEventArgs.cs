using System;

namespace InDoOut_Executable_Core.Loading
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
