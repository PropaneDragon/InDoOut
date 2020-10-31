using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using System.Collections.Generic;

namespace InDoOut_Executable_Core.Programs
{
    public class ProgramHolder : Singleton<ProgramHolder>, IProgramHolder
    {
        public List<IProgram> Programs { get; } = new List<IProgram>();

        public bool AddProgram(IProgram program)
        {
            if (program != null && !ProgramExists(program))
            {
                Programs.Add(program);

                return true;
            }

            return false;
        }

        public IProgram NewProgram()
        {
            var program = new Program();

            return AddProgram(program) ? program : null;
        }

        public bool ProgramExists(IProgram program) => program != null && Programs.Contains(program);

        public bool RemoveProgram(IProgram program)
        {
            if (program != null && ProgramExists(program))
            {
                _ = Programs.RemoveAll(storedProgram => storedProgram == program);

                return true;
            }

            return false;
        }
    }
}
