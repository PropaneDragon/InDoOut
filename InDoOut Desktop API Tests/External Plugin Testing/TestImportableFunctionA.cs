using InDoOut_Core.Entities.Functions;
using InDoOut_Core_Tests;

namespace InDoOut_Desktop_API_Tests.External_Plugin_Testing
{
    public class TestImportableFunctionA : TestFunction
    {
        public IInput AInput1 = null, AInput2 = null;
        public IOutput AOutput1 = null, AOutput2 = null;

        public override string Description => "Importable function A";

        public override string Name => "A";

        public override string Group => "Testing";

        public override string[] Keywords => new[] { "A" };

        public TestImportableFunctionA()
        {
            AInput1 = CreateInput("A Input 1");
            AInput2 = CreateInput("A Input 2");

            AOutput1 = CreateOutput("A Output 1");
            AOutput2 = CreateOutput("A Output 2");
        }
    }
}
