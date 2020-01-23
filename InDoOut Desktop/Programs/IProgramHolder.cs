using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using InDoOut_UI_Common.InterfaceElements;
using System.Collections.Generic;

namespace InDoOut_Desktop.Programs
{
    public interface IProgramHolder : ISingleton<IProgramHolder>, IProgramHandler
    {
        List<IProgram> Programs { get; }

        bool AddProgram(IProgram program);
        bool RemoveProgram(IProgram program);
        bool ProgramExists(IProgram program);
    }
}
