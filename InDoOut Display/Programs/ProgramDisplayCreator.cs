using InDoOut_Core.Entities.Programs;
using InDoOut_Display.UI.Controls.Screens;
using InDoOut_UI_Common.Controls.TaskManager;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Display.Programs
{
    internal class ProgramDisplayCreator : IProgramDisplayCreator
    {
        public ICommonProgramDisplay CreateProgramDisplay(IProgram program) => new ScreenConnections() { AssociatedProgram = program };
    }
}
