using InDoOut_UI_Common.InterfaceElements;
using System;

namespace InDoOut_UI_Common.Events
{
    public class CurrentProgramDisplayEventArgs : EventArgs
    {
        public ICommonProgramDisplay ProgramDisplay { get; private set; } = null;

        public CurrentProgramDisplayEventArgs()
        {
        }

        public CurrentProgramDisplayEventArgs(ICommonProgramDisplay programDisplay)
        {
            ProgramDisplay = programDisplay;
        }
    }
}
