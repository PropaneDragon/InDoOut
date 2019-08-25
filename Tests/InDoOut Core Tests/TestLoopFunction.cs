using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Tests
{
    internal class TestLoopFunction : LoopFunction
    {
        public bool AllItemsCompleteCalled = false;
        public bool PreprocessCalled = false;
        public int ItemsToIterate = 5;

        public override string Description => "A test loop function";

        public override string Name => "Test loop function";

        public override string Group => "Test";

        public override string[] Keywords => new[] { "" };

        protected override void AllItemsComplete()
        {
            AllItemsCompleteCalled = true;
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            return index < ItemsToIterate;
        }

        protected override void PreprocessItems()
        {
            PreprocessCalled = true;
        }
    }
}
