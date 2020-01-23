using InDoOut_Core.Entities.Programs;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface ICommonProgramLoader
    {
        bool DisplayProgram(IProgram program);
        bool UnloadProgram(IProgram program);
    }
}