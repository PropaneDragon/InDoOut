using InDoOut_Core.Entities.Programs;
using InDoOut_UI_Common.Controls.TaskManager;
using InDoOut_UI_Common.InterfaceElements;
using InDoOut_Viewer.UI.Controls.BlockView;

namespace InDoOut_Viewer.Programs
{
    internal class ProgramDisplayCreator : IProgramDisplayCreator
    {
        public ICommonProgramDisplay CreateProgramDisplay(IProgram program) => new BlockView() { AssociatedProgram = program };
    }
}
