using InDoOut_Core.Entities.Programs;
using InDoOut_UI_Common.InterfaceElements;
using System;

namespace InDoOut_UI_Common.Controls.TaskManager
{
    public class NullProgramDisplayCreator : IProgramDisplayCreator
    {
        public ICommonProgramDisplay CreateProgramDisplay(IProgram program)
        {
            throw new Exception("Attempting to use a null program display creator. This needs setting to something else in the calling class!");
        }
    }
}
