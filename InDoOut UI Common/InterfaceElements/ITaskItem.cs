namespace InDoOut_UI_Common.InterfaceElements
{
    public interface ITaskItem
    {
        ICommonProgramDisplay ProgramDisplay { get; }

        void UpdateSnapshotWithTransition();
        void UpdateSnapshot();
    }
}
