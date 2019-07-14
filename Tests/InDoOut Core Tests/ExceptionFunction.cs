using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Tests
{
    public class ExceptionFunction : Function
    {
        public override string Description => throw new Exception();

        public override string Name => throw new Exception();

        public override string Group => throw new Exception();

        public override string[] Keywords => throw new Exception();

        protected override IOutput Started(IInput triggeredBy)
        {
            throw new Exception();
        }
    }
}
