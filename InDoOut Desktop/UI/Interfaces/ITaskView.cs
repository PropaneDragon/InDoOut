namespace InDoOut_Desktop.UI.Interfaces
{
    public interface ITaskView
    {
        IBlockView CurrentBlockView { get; }

        void CreateNewTask(bool bringToFront = false);
        void CreateNewTask(InDoOut_Core.Entities.Programs.IProgram program, bool bringToFront = false);
        void ShowTasks();
        void BringToFront(ITaskItem taskItem);
        void BringToFront(IBlockView blockView);

        bool RemoveTask(ITaskItem task);
    }
}
