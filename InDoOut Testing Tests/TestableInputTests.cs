using InDoOut_Core_Tests;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Testing_Tests
{
    [TestClass]
    public class TestableInputTests
    {
        [TestMethod]
        public void Connections()
        {
            var function = new TestFunction();
            var outputA = function.CreateOutputPublic("Output A");
            var outputB = function.CreateOutputPublic("Output B");
            var outputC = function.CreateOutputPublic("Output C");

            var testableInput = new TestableInput();

            Assert.IsTrue(outputA.Connect(testableInput));
            Assert.IsTrue(outputB.Connect(testableInput));
            Assert.IsTrue(outputC.Connect(testableInput));

            Assert.IsFalse(testableInput.Triggered);
            Assert.AreEqual(0, testableInput.TriggeredCount);
            Assert.IsNull(testableInput.LastTriggeredBy);

            function.OutputToTrigger = outputA;
            function.Trigger(null);
            function.WaitForCompletion(true);

            Assert.IsTrue(testableInput.Triggered);
            Assert.AreEqual(1, testableInput.TriggeredCount);
            Assert.AreEqual(outputA, testableInput.LastTriggeredBy);

            function.OutputToTrigger = outputC;
            function.Trigger(null);
            function.WaitForCompletion(true);

            Assert.IsTrue(testableInput.Triggered);
            Assert.AreEqual(2, testableInput.TriggeredCount);
            Assert.AreEqual(outputC, testableInput.LastTriggeredBy);

            function.OutputToTrigger = outputB;
            function.Trigger(null);
            function.WaitForCompletion(true);

            Assert.IsTrue(testableInput.Triggered);
            Assert.AreEqual(3, testableInput.TriggeredCount);
            Assert.AreEqual(outputB, testableInput.LastTriggeredBy);

            function.OutputToTrigger = outputA;
            function.Trigger(null);
            function.WaitForCompletion(true);

            Assert.IsTrue(testableInput.Triggered);
            Assert.AreEqual(4, testableInput.TriggeredCount);
            Assert.AreEqual(outputA, testableInput.LastTriggeredBy);

            testableInput.Reset(false);

            Assert.IsFalse(testableInput.Triggered);
            Assert.AreEqual(4, testableInput.TriggeredCount);
            Assert.IsNull(testableInput.LastTriggeredBy);

            function.OutputToTrigger = outputB;
            function.Trigger(null);
            function.WaitForCompletion(true);

            Assert.IsTrue(testableInput.Triggered);
            Assert.AreEqual(5, testableInput.TriggeredCount);
            Assert.AreEqual(outputB, testableInput.LastTriggeredBy);

            testableInput.Reset(true);

            Assert.IsFalse(testableInput.Triggered);
            Assert.AreEqual(0, testableInput.TriggeredCount);
            Assert.IsNull(testableInput.LastTriggeredBy);

            function.OutputToTrigger = outputC;
            function.Trigger(null);
            function.WaitForCompletion(true);

            Assert.IsTrue(testableInput.Triggered);
            Assert.AreEqual(1, testableInput.TriggeredCount);
            Assert.AreEqual(outputC, testableInput.LastTriggeredBy);

            testableInput.Reset();

            Assert.IsFalse(testableInput.Triggered);
            Assert.AreEqual(0, testableInput.TriggeredCount);
            Assert.IsNull(testableInput.LastTriggeredBy);
        }
    }
}
