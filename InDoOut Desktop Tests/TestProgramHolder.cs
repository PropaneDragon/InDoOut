using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.Programs;
using System.Collections.Generic;

namespace InDoOut_Desktop_Tests
{
    internal class TestProgramHolder : ProgramHolder
    {
        public List<IProgram> ProgramsPublic => Programs;
    }
}
