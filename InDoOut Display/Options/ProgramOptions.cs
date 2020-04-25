using InDoOut_Core.Options.Types;
using InDoOut_Executable_Core.Options;

namespace InDoOut_Display.Options
{
    internal class ProgramOptions : AbstractProgramOptions
    {
        public CheckableOption StartWithComputerCurrent { get; } = new CheckableOption("Start with computer (current user)", "Starts IDO when the computer starts. This sets the program to start for only you. Other users of this machine are unaffected.", false);
        public HiddenOption LastWindowMaximised { get; } = new HiddenOption("Window maximised");
        public HiddenOption LastWindowX { get; } = new HiddenOption("Window X position");
        public HiddenOption LastWindowY { get; } = new HiddenOption("Window Y position");
        public HiddenOption LastWindowWidth { get; } = new HiddenOption("Window width");
        public HiddenOption LastWindowHeight { get; } = new HiddenOption("Window height");

        public ProgramOptions() : base()
        {
        }

        protected override void HookOptions()
        {
        }
    }
}
