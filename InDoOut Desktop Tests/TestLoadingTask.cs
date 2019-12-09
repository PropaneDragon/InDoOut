using InDoOut_Desktop.Loading;
using InDoOut_Executable_Core.Loading;
using System;
using System.Threading.Tasks;

namespace InDoOut_Desktop_Tests
{
    internal class TestLoadingTask : LoadingTask
    {
        Func<TestLoadingTask, Task<bool>> Action { get; set; } = null;

        public TestLoadingTask(string name, Func<TestLoadingTask, Task<bool>> action)
        {
            Name = name;
            Action = action;
        }

        protected override async Task<bool> RunTaskAsync()
        {
            return Action != null ? await Action.Invoke(this) : false;
        }
    }
}
