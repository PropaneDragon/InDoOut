using InDoOut_Core.Entities.Functions;
using InDoOut_Core_Plugins.Numeric;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Plugins_Tests
{
    [TestClass]
    public class NumericTests
    {
        [TestMethod]
        public void EqualTo()
        {
            var function = new EqualToFunction();

            Assert.IsTrue(function.SetPropertyValue("First number", "10"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "20"));

            var connection = new TestableInput();

            var equal = function.GetOutputByName("Numbers are equal");
            var notEqual = function.GetOutputByName("Numbers are different");

            Assert.IsTrue(equal.Connect(connection));
            Assert.IsTrue(notEqual.Connect(connection));

            TriggerFunctionAndCheckOutput(function, notEqual, connection, 1);

            Assert.IsTrue(function.SetPropertyValue("First number", "20"));
            TriggerFunctionAndCheckOutput(function, equal, connection, 2);

            Assert.IsTrue(function.SetPropertyValue("First number", "21"));
            TriggerFunctionAndCheckOutput(function, notEqual, connection, 3);

            Assert.IsTrue(function.SetPropertyValue("First number", "19"));
            TriggerFunctionAndCheckOutput(function, notEqual, connection, 4);

            Assert.IsTrue(function.SetPropertyValue("First number", "-1"));
            TriggerFunctionAndCheckOutput(function, notEqual, connection, 5);

            Assert.IsTrue(function.SetPropertyValue("First number", "-20"));
            TriggerFunctionAndCheckOutput(function, notEqual, connection, 6);

            Assert.IsTrue(function.SetPropertyValue("First number", "cabbage"));
            TriggerFunctionAndCheckOutput(function, notEqual, connection, 7);

            Assert.IsTrue(function.SetPropertyValue("First number", "20"));
            TriggerFunctionAndCheckOutput(function, equal, connection, 8);

            Assert.IsTrue(function.SetPropertyValue("First number", "200"));
            TriggerFunctionAndCheckOutput(function, notEqual, connection, 9);

            Assert.IsTrue(function.SetPropertyValue("First number", "20.00001"));
            TriggerFunctionAndCheckOutput(function, notEqual, connection, 10);
        }

        [TestMethod]
        public void GreaterThan()
        {
            var function = new GreaterThanFunction();

            Assert.IsTrue(function.SetPropertyValue("First number", "10"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "20"));

            var connection = new TestableInput();

            var more = function.GetOutputByName("First number is more");
            var notMore = function.GetOutputByName("First number not more");

            Assert.IsTrue(more.Connect(connection));
            Assert.IsTrue(notMore.Connect(connection));

            TriggerFunctionAndCheckOutput(function, notMore, connection, 1);

            Assert.IsTrue(function.SetPropertyValue("First number", "20"));
            TriggerFunctionAndCheckOutput(function, notMore, connection, 2);

            Assert.IsTrue(function.SetPropertyValue("First number", "19.999"));
            TriggerFunctionAndCheckOutput(function, notMore, connection, 3);

            Assert.IsTrue(function.SetPropertyValue("First number", "-21"));
            TriggerFunctionAndCheckOutput(function, notMore, connection, 4);

            Assert.IsTrue(function.SetPropertyValue("First number", "0"));
            TriggerFunctionAndCheckOutput(function, notMore, connection, 5);

            Assert.IsTrue(function.SetPropertyValue("First number", "-1"));
            TriggerFunctionAndCheckOutput(function, notMore, connection, 6);

            Assert.IsTrue(function.SetPropertyValue("First number", "21"));
            TriggerFunctionAndCheckOutput(function, more, connection, 7);

            Assert.IsTrue(function.SetPropertyValue("First number", "22"));
            TriggerFunctionAndCheckOutput(function, more, connection, 8);

            Assert.IsTrue(function.SetPropertyValue("First number", "200"));
            TriggerFunctionAndCheckOutput(function, more, connection, 9);

            Assert.IsTrue(function.SetPropertyValue("First number", "190"));
            TriggerFunctionAndCheckOutput(function, more, connection, 10);

            Assert.IsTrue(function.SetPropertyValue("First number", "20.1"));
            TriggerFunctionAndCheckOutput(function, more, connection, 11);
        }

        private void TriggerFunctionAndCheckOutput(IFunction function, IOutput output, TestableInput connectedInput, int expectedCount)
        {
            function.Trigger(null);
            function.WaitForCompletion(true);

            Assert.AreEqual(output, connectedInput.LastTriggeredBy);
            Assert.AreEqual(expectedCount, connectedInput.TriggeredCount);
        }
    }
}
