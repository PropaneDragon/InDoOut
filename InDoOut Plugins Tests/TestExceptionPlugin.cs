using InDoOut_Plugins.Core;
using System;

namespace InDoOut_Plugins_Tests
{
    internal class TestExceptionPlugin : Plugin
    {
        public override string Name => throw new Exception();

        public override string Description => throw new Exception();

        public override string Author => throw new Exception();
    }
}
