using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using System.Collections.Generic;

namespace InDoOut_Executable_Core.Programs
{
    public interface IProgramHolder : ISingleton<IProgramHolder>, IProgramHandler
    {
        List<IProgram> Programs { get; }

        bool AddProgram(IProgram program);
        bool RemoveProgram(IProgram program);
        bool ProgramExists(IProgram program);
    }
}
