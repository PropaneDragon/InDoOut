using InDoOut_Desktop.Actions;
using InDoOut_UI_Common.Actions;

namespace InDoOut_Desktop_Tests
{
    internal class TestActionHandler : ActionHandler
    {
        public TestActionHandler(IAction defaultAction = null) : base(defaultAction)
        {
        }
    }
}
