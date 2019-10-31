using InDoOut_Plugins.Options.Types;

namespace InDoOut_Desktop.Options
{
    public static class ProgramSettings
    {
        public static CheckableOption START_WITH_COMPUTER = new CheckableOption("Start with computer", "Starts IDO when the computer starts.", true);
        public static CheckableOption START_IN_BACKGROUND = new CheckableOption("Start in the background", "Starts IDO minimised.", false);
    }
}
