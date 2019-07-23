using InDoOut_Desktop_API.Plugins;
using System;

namespace InDoOut_Desktop_API_Tests
{
    class TestExceptionPlugin : Plugin
    {
        public override string Name => throw new Exception();

        public override string Description => throw new Exception();

        public override string Author => throw new Exception();
    }
}
