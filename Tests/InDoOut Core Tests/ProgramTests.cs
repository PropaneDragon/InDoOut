using InDoOut_Core.Entities.Programs;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void AddFunctions()
        {
            var program = new Program();

            Assert.AreEqual(0, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            var function = new TestFunction();

            Assert.IsTrue(program.AddFunction(function));

            Assert.AreEqual(1, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            Assert.IsFalse(program.AddFunction(function));
            Assert.IsFalse(program.AddFunction(null));

            Assert.AreEqual(1, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            var startFunction = new TestStartFunction();

            Assert.IsTrue(program.AddFunction(startFunction));

            Assert.AreEqual(2, program.Functions.Count);
            Assert.AreEqual(1, program.StartFunctions.Count);
            Assert.AreEqual(startFunction, program.StartFunctions[0]);

            Assert.IsFalse(program.AddFunction(startFunction));
        }

        [TestMethod]
        public void RemoveFunctions()
        {
            var program = new Program();

            Assert.AreEqual(0, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            var function = new TestFunction();

            Assert.IsTrue(program.AddFunction(function));
            Assert.AreEqual(1, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            var function2 = new TestFunction();

            Assert.IsTrue(program.AddFunction(function2));
            Assert.AreEqual(2, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            var startFunction = new TestStartFunction();

            Assert.IsTrue(program.AddFunction(startFunction));
            Assert.AreEqual(3, program.Functions.Count);
            Assert.AreEqual(1, program.StartFunctions.Count);

            Assert.IsFalse(program.RemoveFunction(null));
            Assert.IsFalse(program.RemoveFunction(new TestFunction()));
            Assert.IsFalse(program.RemoveFunction(new TestStartFunction()));
            Assert.AreEqual(3, program.Functions.Count);
            Assert.AreEqual(1, program.StartFunctions.Count);

            Assert.IsTrue(program.RemoveFunction(function2));
            Assert.AreEqual(2, program.Functions.Count);
            Assert.AreEqual(1, program.StartFunctions.Count);

            Assert.IsTrue(program.RemoveFunction(startFunction));
            Assert.AreEqual(1, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            Assert.IsTrue(program.AddFunction(function2));
            Assert.AreEqual(2, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            Assert.IsTrue(program.RemoveFunction(function));
            Assert.AreEqual(1, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);

            Assert.IsTrue(program.RemoveFunction(function2));
            Assert.AreEqual(0, program.Functions.Count);
            Assert.AreEqual(0, program.StartFunctions.Count);
        }

        [TestMethod]
        public void StartFunction()
        {
            var program = new Program();
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(10)));
            var startFunction = new TestStartFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(10)));

            var output = startFunction.CreateOutputPublic();
            var input = function.CreateInputPublic();

            startFunction.OutputToTrigger = output;

            Assert.IsTrue(output.Connect(input));
            Assert.IsTrue(program.AddFunction(function));
            Assert.AreEqual(1, program.Functions.Count);

            program.Trigger(null);

            Assert.IsFalse(function.WaitForCompletion(TimeSpan.FromMilliseconds(100), true));
            Assert.IsFalse(startFunction.HasRun);
            Assert.IsFalse(function.HasRun);

            Assert.IsTrue(program.AddFunction(startFunction));
            Assert.AreEqual(2, program.Functions.Count);
            Assert.AreEqual(1, program.StartFunctions.Count);

            program.Trigger(null);

            Assert.IsTrue(startFunction.WaitForCompletion(TimeSpan.FromMilliseconds(150)));
            Assert.IsTrue(function.WaitForCompletion(TimeSpan.FromMilliseconds(150)));
            Assert.IsTrue(startFunction.HasRun);
            Assert.IsTrue(function.HasRun);
        }

        [TestMethod]
        public void MuiltipleStartFunctions()
        {
            var program = new Program();
            var startFunctionA = new TestStartFunction();
            var startFunctionB = new TestStartFunction();
            var functionA = new TestFunction();
            var functionB = new TestFunction();
            var functionC = new TestFunction();

            var outputA = startFunctionA.CreateOutputPublic();
            var outputB = startFunctionB.CreateOutputPublic();
            var inputA = functionA.CreateInputPublic();
            var inputB = functionB.CreateInputPublic();
            var inputC = functionC.CreateInputPublic();

            startFunctionA.OutputToTrigger = outputA;
            startFunctionB.OutputToTrigger = outputB;

            Assert.IsTrue(outputA.Connect(inputA));
            Assert.IsTrue(outputB.Connect(inputB));
            Assert.IsTrue(outputB.Connect(inputC));

            Assert.IsTrue(program.AddFunction(functionA));
            Assert.IsTrue(program.AddFunction(functionB));
            Assert.IsTrue(program.AddFunction(functionC));

            Assert.IsTrue(program.AddFunction(startFunctionA));
            Assert.AreEqual(4, program.Functions.Count);
            Assert.AreEqual(1, program.StartFunctions.Count);

            Assert.IsFalse(startFunctionA.HasRun);
            Assert.IsFalse(startFunctionB.HasRun);
            Assert.IsFalse(functionA.HasRun);
            Assert.IsFalse(functionB.HasRun);
            Assert.IsFalse(functionC.HasRun);

            program.Trigger(null);

            var startTime = DateTime.UtcNow;

            while (program.Running && DateTime.UtcNow < startTime.Add(TimeSpan.FromSeconds(1)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(startFunctionA.HasRun);
            Assert.IsFalse(startFunctionB.HasRun);
            Assert.IsTrue(functionA.HasRun);
            Assert.IsFalse(functionB.HasRun);
            Assert.IsFalse(functionC.HasRun);

            startFunctionA.HasRun = false;
            functionA.HasRun = false;

            Assert.IsFalse(startFunctionA.HasRun);
            Assert.IsFalse(startFunctionB.HasRun);
            Assert.IsFalse(functionA.HasRun);
            Assert.IsFalse(functionB.HasRun);
            Assert.IsFalse(functionC.HasRun);

            Assert.IsTrue(program.AddFunction(startFunctionB));

            program.Trigger(null);
            startTime = DateTime.UtcNow;

            while (program.Running && DateTime.UtcNow < startTime.Add(TimeSpan.FromSeconds(1)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(startFunctionA.HasRun);
            Assert.IsTrue(startFunctionB.HasRun);
            Assert.IsTrue(functionA.HasRun);
            Assert.IsTrue(functionB.HasRun);
            Assert.IsTrue(functionC.HasRun);
        }

        [TestMethod]
        public void EndToEnd()
        {
            var program = new Program();
            var startFunctionA = new TestStartFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(5)));
            var functionB = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(5)));
            var functionC = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(5)));
            var functionD = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(5)));
            var functionE = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(5)));

            var outputA = startFunctionA.CreateOutputPublic();
            var outputB = functionB.CreateOutputPublic();
            var outputC = functionC.CreateOutputPublic();
            var outputD = functionD.CreateOutputPublic();

            var inputB = functionB.CreateInputPublic();
            var inputC = functionC.CreateInputPublic();
            var inputD = functionD.CreateInputPublic();
            var inputE = functionE.CreateInputPublic();

            startFunctionA.OutputToTrigger = outputA;
            functionB.OutputToTrigger = outputB;
            functionC.OutputToTrigger = outputC;
            functionD.OutputToTrigger = outputD;

            Assert.IsTrue(outputA.Connect(inputB));
            Assert.IsTrue(outputB.Connect(inputC));
            Assert.IsTrue(outputC.Connect(inputD));
            Assert.IsTrue(outputD.Connect(inputE));

            Assert.IsTrue(program.AddFunction(startFunctionA));
            Assert.IsTrue(program.AddFunction(functionB));
            Assert.IsTrue(program.AddFunction(functionC));
            Assert.IsTrue(program.AddFunction(functionD));
            Assert.IsTrue(program.AddFunction(functionE));

            Assert.IsFalse(startFunctionA.HasRun);
            Assert.IsFalse(functionB.HasRun);
            Assert.IsFalse(functionC.HasRun);
            Assert.IsFalse(functionD.HasRun);
            Assert.IsFalse(functionE.HasRun);

            program.Trigger(null);

            var startTime = DateTime.UtcNow;

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            while (program.Running && DateTime.UtcNow < startTime.Add(TimeSpan.FromSeconds(1)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(startFunctionA.HasRun);
            Assert.IsTrue(functionB.HasRun);
            Assert.IsTrue(functionC.HasRun);
            Assert.IsTrue(functionD.HasRun);
            Assert.IsTrue(functionE.HasRun);
        }
    }
}
