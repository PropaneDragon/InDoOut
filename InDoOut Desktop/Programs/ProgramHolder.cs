using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Instancing;
using System.Collections.Generic;

namespace InDoOut_Desktop.Programs
{
    internal class ProgramHolder : Singleton<ProgramHolder>, IProgramHolder
    {
        private readonly List<IProgram> _programs = new List<IProgram>();

        protected List<IProgram> Programs => _programs;

        public bool AddProgram(IProgram program)
        {
            if (program != null && !ProgramExists(program))
            {
                _programs.Add(program);

                return true;
            }

            return false;
        }

        public IProgram NewProgram()
        {
            var program = new Program();

            return AddProgram(program) ? program : null;
        }

        public bool ProgramExists(IProgram program)
        {
            return program != null && _programs.Contains(program);
        }

        public bool RemoveProgram(IProgram program)
        {
            if (program != null && ProgramExists(program))
            {
                _programs.RemoveAll(storedProgram => storedProgram == program);

                return true;
            }

            return false;
        }
    }
}
