using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InDoOut_Desktop.Loading
{
    internal abstract class LoadingTask : ILoadingTask
    {
        private object _nameLock = new object();

        private string _name = null;
        private List<ILoadingTask> _childTasks = new List<ILoadingTask>();

        public event EventHandler<LoadingTaskEventArgs> NameChanged;

        public TaskState State { get; private set; } = TaskState.NotRun;

        public string Name
        {
            get { lock (_nameLock) return _name; }
            set
            {
                lock (_nameLock) _name = value;
                NameChanged?.Invoke(this, new LoadingTaskEventArgs<LoadingTask>(this));
            }
        }

        public bool Add(ILoadingTask task)
        {
            if (task != null)
            {
                _childTasks.Add(task);

                return true;
            }

            return false;
        }

        public bool Remove(ILoadingTask task)
        {
            if (task != null && _childTasks.Contains(task))
            {
                _ = _childTasks.Remove(task);

                return true;
            }

            return false;
        }

        public async Task<bool> RunAsync()
        {
            var result = true;

            State = TaskState.Running;

            foreach (var childTask in _childTasks)
            {
                result = result && await childTask.RunAsync();
            }

            result = result && await RunTaskAsync();

            State = TaskState.HasRun;

            return result;
        }

        protected abstract Task<bool> RunTaskAsync();
    }
}
