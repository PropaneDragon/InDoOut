using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using System.Collections.Generic;

namespace InDoOut_Desktop.Programs
{
    public interface IProgramHolder : ISingleton<IProgramHolder>
    {
        List<IProgram> Programs { get; }

        bool AddProgram(IProgram program);
        bool RemoveProgram(IProgram program);
        bool ProgramExists(IProgram program);

        IProgram NewProgram();
    }
}
