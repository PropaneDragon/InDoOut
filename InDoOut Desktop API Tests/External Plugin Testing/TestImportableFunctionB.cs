using InDoOut_Core.Entities.Functions;
using InDoOut_Core_Tests;

namespace InDoOut_Desktop_API_Tests.External_Plugin_Testing
{
    public class TestImportableFunctionB : TestFunction
    {
        public IInput BInput1 = null, BInput2 = null;
        public IOutput BOutput1 = null, BOutput2 = null;
        public IProperty BProperty1 = null, BProperty2 = null;
        public IResult BResult1 = null, BResult2 = null;

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

            BProperty1 = AddProperty(new Property<string>("B Property 1", "B Property 1 Description"));
            BProperty2 = AddProperty(new Property<int>("B Property 2", "B Property 2 Description"));

            BResult1 = AddResult(new Result("B Result 1", "B Result 1 Description"));
            BResult2 = AddResult(new Result("B Result 2", "B Result 2 Description"));
        }
    }
}
