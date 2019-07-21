using InDoOut_Core.Entities.Functions;
using InDoOut_Core_Tests;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Testing_Tests
{
    [TestClass]
    public class FunctionTestingExtensionsTests
    {
        [TestMethod]
        public void WaitForCompletion()
        {
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(50)));
            function.Trigger(null);

            var startTime = DateTime.UtcNow;

            Assert.IsTrue(function.WaitForCompletion(TimeSpan.FromMilliseconds(80)));

            var waitTime = DateTime.UtcNow - startTime;

            Assert.AreEqual(50, waitTime.TotalMilliseconds, 10, $"Total time: {waitTime.TotalMilliseconds}ms");

            function = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(2)));
            function.Trigger(null);

            startTime = DateTime.UtcNow;

            Assert.IsFalse(function.WaitForCompletion(TimeSpan.FromMilliseconds(60)));

            waitTime = DateTime.UtcNow - startTime;

            Assert.AreEqual(60, waitTime.TotalMilliseconds, 10, $"Total time: {waitTime.TotalMilliseconds}ms");
        }

        [TestMethod]
        public void WaitForStartAndCompletion()
        {
            var startFunction = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(50)));
            var endFunction = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(50)));

            var output = startFunction.CreateOutputPublic(OutputType.Neutral);
            var input = endFunction.CreateInputPublic();

            Assert.IsTrue(output.Connect(input));

            startFunction.OutputToTrigger = output;
            startFunction.Trigger(null);

            var startTime = DateTime.UtcNow;

            endFunction.WaitForCompletion(TimeSpan.FromSeconds(1), true);

            var waitTime = DateTime.UtcNow - startTime;

            Assert.AreEqual(100, waitTime.TotalMilliseconds, 10, $"Total time: {waitTime.TotalMilliseconds}");
        }
    }
}
