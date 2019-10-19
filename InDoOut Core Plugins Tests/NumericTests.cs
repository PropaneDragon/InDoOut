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

        [TestMethod]
        public void IsANumber()
        {
            var function = new IsANumberFunction();
            var connection = new TestableInput();

            var isNumber = function.GetOutputByName("Is a number");
            var notNumber = function.GetOutputByName("Not a number");

            Assert.IsTrue(isNumber.Connect(connection));
            Assert.IsTrue(notNumber.Connect(connection));

            Assert.IsTrue(function.SetPropertyValue("Value", "1"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 1);

            Assert.IsTrue(function.SetPropertyValue("Value", "-1"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 2);

            Assert.IsTrue(function.SetPropertyValue("Value", "0"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 3);

            Assert.IsTrue(function.SetPropertyValue("Value", "23.23456"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 4);

            Assert.IsTrue(function.SetPropertyValue("Value", "123456789"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 5);

            Assert.IsTrue(function.SetPropertyValue("Value", "01516"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 6);

            Assert.IsTrue(function.SetPropertyValue("Value", "0.12513"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 7);

            Assert.IsTrue(function.SetPropertyValue("Value", "-0"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 8);

            Assert.IsTrue(function.SetPropertyValue("Value", "-123456789"));
            TriggerFunctionAndCheckOutput(function, isNumber, connection, 9);

            Assert.IsTrue(function.SetPropertyValue("Value", "NaN"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 10);

            Assert.IsTrue(function.SetPropertyValue("Value", "actually not a number"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 11);

            Assert.IsTrue(function.SetPropertyValue("Value", "Contains 0 a number"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 12);

            Assert.IsTrue(function.SetPropertyValue("Value", "1 Starts with a number"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 13);

            Assert.IsTrue(function.SetPropertyValue("Value", "Ends with a number 2"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 14);

            Assert.IsTrue(function.SetPropertyValue("Value", "a325235"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 15);

            Assert.IsTrue(function.SetPropertyValue("Value", "a32562346b"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 16);

            Assert.IsTrue(function.SetPropertyValue("Value", "2352352c"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 17);

            Assert.IsTrue(function.SetPropertyValue("Value", "0x01"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 18);

            Assert.IsTrue(function.SetPropertyValue("Value", "f0f0f0"));
            TriggerFunctionAndCheckOutput(function, notNumber, connection, 19);
        }

        [TestMethod]
        public void LessThan()
        {
            var function = new LessThanFunction();
            var connection = new TestableInput();

            var isLess = function.GetOutputByName("First number is less");
            var notLess = function.GetOutputByName("First number not less");

            Assert.IsTrue(isLess.Connect(connection));
            Assert.IsTrue(notLess.Connect(connection));

            Assert.IsTrue(function.SetPropertyValue("First number", "10"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "20"));
            TriggerFunctionAndCheckOutput(function, isLess, connection, 1);

            Assert.IsTrue(function.SetPropertyValue("First number", "-20"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "-10"));
            TriggerFunctionAndCheckOutput(function, isLess, connection, 2);

            Assert.IsTrue(function.SetPropertyValue("First number", "10"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "200000"));
            TriggerFunctionAndCheckOutput(function, isLess, connection, 3);

            Assert.IsTrue(function.SetPropertyValue("First number", "19.99999999"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "20"));
            TriggerFunctionAndCheckOutput(function, isLess, connection, 4);

            Assert.IsTrue(function.SetPropertyValue("First number", "20"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "20.000001"));
            TriggerFunctionAndCheckOutput(function, isLess, connection, 5);

            Assert.IsTrue(function.SetPropertyValue("First number", "10"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "10"));
            TriggerFunctionAndCheckOutput(function, notLess, connection, 6);

            Assert.IsTrue(function.SetPropertyValue("First number", "11"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "10"));
            TriggerFunctionAndCheckOutput(function, notLess, connection, 7);

            Assert.IsTrue(function.SetPropertyValue("First number", "10.0000001"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "10"));
            TriggerFunctionAndCheckOutput(function, notLess, connection, 8);

            Assert.IsTrue(function.SetPropertyValue("First number", "-10"));
            Assert.IsTrue(function.SetPropertyValue("Second number", "-11"));
            TriggerFunctionAndCheckOutput(function, notLess, connection, 9);
        }

        private void TriggerFunctionAndCheckOutput(IFunction function, IOutput output, TestableInput connectedInput, int expectedCount)
        {
            function.Trigger(null);
            function.WaitForCompletion(true);
            connectedInput.WaitForCompletion();

            Assert.AreEqual(output, connectedInput.LastTriggeredBy);
            Assert.AreEqual(expectedCount, connectedInput.TriggeredCount);
        }
    }
}
