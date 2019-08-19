using InDoOut_Core.Entities.Functions;
using InDoOut_Core_Tests;

namespace InDoOut_Desktop_API_Tests.External_Plugin_Testing
{
    public class TestImportableFunctionB : TestFunction
    {
        public IInput BInput1 = null, BInput2 = null;
        public IOutput BOutput1 = null, BOutput2 = null;

        public override string Description => "Importable function B";

        public override string Name => "B";

        public override string Group => "Testing";

        public override string[] Keywords => new[] { "B" };

        public TestImportableFunctionB()
        {
            BInput1 = CreateInput("B Input 1");
            BInput2 = CreateInput("B Input 2");

            BOutput1 = CreateOutput("B Output 1");
            BOutput2 = CreateOutput("B Output 2");
        }
    }
}
