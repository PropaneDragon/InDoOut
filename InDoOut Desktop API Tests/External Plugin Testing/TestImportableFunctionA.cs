using InDoOut_Core.Entities.Functions;
using InDoOut_Core_Tests;

namespace InDoOut_Desktop_API_Tests.External_Plugin_Testing
{
    public class TestImportableFunctionA : TestFunction
    {
        public override string Description => "Importable function A";

        public override string Name => "A";

        public override string Group => "Testing";

        public override string[] Keywords => new[] { "A" };
    }
}
