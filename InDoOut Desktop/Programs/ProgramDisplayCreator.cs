using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.UI.Controls.BlockView;
using InDoOut_UI_Common.Controls.TaskManager;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Desktop.Programs
{
    internal class ProgramDisplayCreator : IProgramDisplayCreator
    {
        public ICommonProgramDisplay CreateProgramDisplay(IProgram program) => new BlockView() { AssociatedProgram = program };
    }
}
