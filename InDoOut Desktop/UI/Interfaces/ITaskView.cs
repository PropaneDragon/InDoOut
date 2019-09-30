namespace InDoOut_Desktop.UI.Interfaces
{
    public interface ITaskView
    {
        IBlockView CurrentBlockView { get; }

        void CreateNewTask();
        void CreateNewTask(InDoOut_Core.Entities.Programs.IProgram program);
        void ShowTasks();
        void BringToFront(ITaskItem taskItem);
        void BringToFront(IBlockView blockView);
    }
}
