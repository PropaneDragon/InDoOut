using InDoOut_Core_Tests;

namespace InDoOut_Desktop_API_Tests.External_Plugin_Testing
{
    internal class TestPrivateFunction : TestFunction
    {
        public override string Description => "Unimportable function";

        public override string Name => "Nah";

        public override string Group => "Testing";

        public override string[] Keywords => new[] { "Unimportable" };
    }
}
