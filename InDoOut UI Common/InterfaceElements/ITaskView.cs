using InDoOut_Core.Entities.Programs;
using InDoOut_UI_Common.Events;
using System;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface ITaskView
    {
        event EventHandler<CurrentProgramDisplayEventArgs> OnProgramDisplayChanged;

        ICommonProgramDisplay CurrentProgramDisplay { get; }

        void CreateNewTask(bool bringToFront = false);
        void CreateNewTask(IProgram program, bool bringToFront = false);
        void ShowTasks();
        void ToggleTasks();
        void BringToFront(ITaskItem taskItem);
        void BringToFront(ICommonProgramDisplay programDisplay);

        bool RemoveTask(ITaskItem task);
    }
}
