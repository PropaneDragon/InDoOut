using InDoOut_Core.Entities.Programs;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Controls.TaskManager
{
    public interface IProgramDisplayCreator
    {
        ICommonProgramDisplay CreateProgramDisplay(IProgram program);
    }
}
