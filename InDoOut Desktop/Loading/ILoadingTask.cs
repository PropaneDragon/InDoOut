using InDoOut_Core.Basic;
using System;
using System.Threading.Tasks;

namespace InDoOut_Desktop.Loading
{
    public enum TaskState
    {
        NotRun,
        Running,
        HasRun
    }

    public interface ILoadingTask : INamed
    {
        event EventHandler<LoadingTaskEventArgs> NameChanged;

        TaskState State { get; }
        bool Add(ILoadingTask task);
        bool Remove(ILoadingTask task);
        Task<bool> RunAsync();
    }
}
