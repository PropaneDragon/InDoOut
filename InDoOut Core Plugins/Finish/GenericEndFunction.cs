using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Finish
{
    public class GenericEndFunction : EndFunction
    {
        public override string Description => "Returns a value back to the caller.";

        public override string Name => "Return value";

        public override string[] Keywords => new[] { "return", "code", "retval", "result", "retcode" };
    }
}
