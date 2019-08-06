using InDoOut_Desktop.Actions;

namespace InDoOut_Desktop_Tests
{
    internal class TestActionHandler : ActionHandler
    {
        public TestActionHandler(IAction defaultAction = null) : base(defaultAction)
        {
        }
    }
}
