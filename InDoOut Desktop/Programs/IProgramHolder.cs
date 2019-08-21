using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;

namespace InDoOut_Desktop.Programs
{
    internal interface IProgramHolder : ISingleton<IProgramHolder>
    {
        bool AddProgram(IProgram program);
        bool RemoveProgram(IProgram program);
        bool ProgramExists(IProgram program);

        IProgram NewProgram();
    }
}
