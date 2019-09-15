using InDoOut_Core.Entities.Programs;
using InDoOut_Core_Plugins.Maths;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InDoOut_Core_Plugins_Tests
{
    [TestClass]
    public class MathsTests
    {
        [TestMethod]
        public void Add()
        {
            var program = new Program();
            var function = new AddFunction();

            _ = program.AddFunction(function);

            Assert.AreEqual(2, function.Properties.Count);
            Assert.AreEqual(1 + function.Properties.Count, function.Results.Count);

            TestArithmetic(function, (first, second) => first + second);
        }

        [TestMethod]
        public void Subtract()
        {
            var program = new Program();
            var function = new SubtractFunction();

            _ = program.AddFunction(function);

            Assert.AreEqual(2, function.Properties.Count);
            Assert.AreEqual(1 + function.Properties.Count, function.Results.Count);

            TestArithmetic(function, (first, second) => first - second);
        }

        [TestMethod]
        public void Multiply()
        {
            var program = new Program();
            var function = new MultiplyFunction();

            _ = program.AddFunction(function);

            Assert.AreEqual(2, function.Properties.Count);
            Assert.AreEqual(1 + function.Properties.Count, function.Results.Count);

            TestArithmetic(function, (first, second) => first * second);
        }

        [TestMethod]
        public void Divide()
        {
            var program = new Program();
            var function = new DivideFunction();

            _ = program.AddFunction(function);

            Assert.AreEqual(2, function.Properties.Count);
            Assert.AreEqual(1 + function.Properties.Count, function.Results.Count);

            TestArithmetic(function, (first, second) => first / second);
        }

        private void TestArithmetic(AbstractPairArithmeticFunction arithmeticFunction, Func<double, double, double> function)
        {
            var result = arithmeticFunction.Results[2];
            var firstNumber = arithmeticFunction.Properties[0];
            var secondNumber = arithmeticFunction.Properties[1];

            Assert.AreEqual("First number", firstNumber.Name);
            Assert.AreEqual("Second number", secondNumber.Name);

            arithmeticFunction.Trigger(null);
            arithmeticFunction.WaitForCompletion();

            Assert.AreEqual(0, result.ValueAs<double>());

            firstNumber.RawValue = "10";
            secondNumber.RawValue = "15";

            arithmeticFunction.Trigger(null);
            arithmeticFunction.WaitForCompletion();

            Assert.AreEqual(function.Invoke(10, 15), result.ValueAs<double>(), 0.1);

            firstNumber.RawValue = "-10";
            secondNumber.RawValue = "15";

            arithmeticFunction.Trigger(null);
            arithmeticFunction.WaitForCompletion();

            Assert.AreEqual(function.Invoke(-10, 15), result.ValueAs<double>(), 0.1);

            firstNumber.RawValue = "10.225";
            secondNumber.RawValue = "15.822";

            arithmeticFunction.Trigger(null);
            arithmeticFunction.WaitForCompletion();

            Assert.AreEqual(function.Invoke(10.225, 15.822), result.ValueAs<double>(), 0.001);
        }
    }
}
