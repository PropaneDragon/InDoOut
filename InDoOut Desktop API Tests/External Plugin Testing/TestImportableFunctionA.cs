using InDoOut_Core.Entities.Functions;
using InDoOut_Core_Tests;

namespace InDoOut_Desktop_API_Tests.External_Plugin_Testing
{
    public class TestImportableFunctionA : TestFunction
    {
        public IInput AInput1 = null, AInput2 = null;
        public IOutput AOutput1 = null, AOutput2 = null;
        public IProperty AProperty1 = null, AProperty2 = null;
        public IResult AResult1 = null, AResult2 = null;

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

            AProperty1 = AddProperty(new Property<int>("A Property 1", "A Property 1 Description"));
            AProperty2 = AddProperty(new Property<string>("A Property 2", "A Property 2 Description"));

            AResult1 = AddResult(new Result("A Result 1", "A Result 1 Description"));
            AResult2 = AddResult(new Result("A Result 2", "A Result 2 Description"));
        }
    }
}
