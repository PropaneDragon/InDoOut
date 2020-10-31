using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Start
{
    public class GenericStartFunction : StartFunction
    {
        public override string Description => "The start of a program. This is called when the program is started through either the editor, command or schedule.";

        public override string Name => "Program start";

        public override string[] Keywords => new[] { "Begin", "Beginning", "Starting", "Front", "Source" };

        public GenericStartFunction() : base()
        {

        }

        protected override IOutput Started(IInput triggeredBy) => OutputStart;
    }
}
