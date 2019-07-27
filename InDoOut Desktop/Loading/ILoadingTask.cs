using InDoOut_Core.Basic;
using System;
using System.Threading.Tasks;

namespace InDoOut_Desktop.Loading
{
    enum TaskState
    {
        NotRun,
        Running,
        HasRun
    }

    internal interface ILoadingTask : INamed
    {
        event EventHandler<LoadingTaskEventArgs> OnNameChanged;

        TaskState State { get; }
        bool Add(ILoadingTask task);
        bool Remove(ILoadingTask task);
        Task<bool> RunAsync();
    }
}
