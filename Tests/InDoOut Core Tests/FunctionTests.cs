using InDoOut_Core.Entities.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void CreateInputsOutputs()
        {
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(5)));

            Assert.AreEqual(0, function.Inputs.Count);
            Assert.AreEqual(0, function.Outputs.Count);

            var input = function.CreateInputPublic();
            var output = function.CreateOutputPublic();

            Assert.AreEqual(1, function.Inputs.Count);
            Assert.AreEqual(1, function.Outputs.Count);
            Assert.IsNotNull(input);
            Assert.IsNotNull(output);

            input = function.CreateInputPublic();
            output = function.CreateOutputPublic();

            Assert.AreEqual(1, function.Inputs.Count);
            Assert.AreEqual(1, function.Outputs.Count);
            Assert.IsNull(input);
            Assert.IsNull(output);

            input = function.CreateInputPublic("A");
            output = function.CreateOutputPublic("A");

            Assert.AreEqual(2, function.Inputs.Count);
            Assert.AreEqual(2, function.Outputs.Count);
            Assert.IsNotNull(input);
            Assert.IsNotNull(output);
            Assert.AreEqual("A", input.Name);
            Assert.AreEqual("A", output.Name);

            input = function.CreateInputPublic("B");
            output = function.CreateOutputPublic("C");

            Assert.AreEqual(3, function.Inputs.Count);
            Assert.AreEqual(3, function.Outputs.Count);
            Assert.IsNotNull(input);
            Assert.IsNotNull(output);
            Assert.AreEqual("B", input.Name);
            Assert.AreEqual("C", output.Name);
        }

        [TestMethod]
        public void ProcessOutput()
        {
            var startFunction = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(1)));
            var outputA = startFunction.CreateOutputPublic("A");
            var outputB = startFunction.CreateOutputPublic("B");

            var endFunction = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(1)));
            var inputA = endFunction.CreateInputPublic("A");
            var inputB = endFunction.CreateInputPublic("B");

            outputA.Connect(inputA);
            outputB.Connect(inputB);

            Assert.IsNull(endFunction.LastInput);

            startFunction.OutputToTrigger = outputA;
            startFunction.Trigger(null);

            Thread.Sleep(TimeSpan.FromMilliseconds(5));

            Assert.AreEqual(inputA, endFunction.LastInput);

            startFunction.OutputToTrigger = outputB;
            startFunction.Trigger(null);

            Thread.Sleep(TimeSpan.FromMilliseconds(5));

            Assert.AreEqual(inputB, endFunction.LastInput);
        }

        [TestMethod]
        public void ProcessDuration()
        {
            CheckDuration(TimeSpan.FromSeconds(1));
            CheckDuration(TimeSpan.FromMilliseconds(50));
            CheckDuration(TimeSpan.FromMilliseconds(5));
            CheckDuration(TimeSpan.FromMilliseconds(16.2));
            CheckDuration(TimeSpan.FromMilliseconds(200));
            CheckDuration(TimeSpan.FromSeconds(3.1));
        }

        private void CheckDuration(TimeSpan duration)
        {
            var function = new TestFunction(() => Thread.Sleep(duration));

            function.Trigger(null);

            var startTime = DateTime.UtcNow;

            while (!function.Running && function.State != State.Processing && DateTime.UtcNow - startTime < duration.Add(TimeSpan.FromSeconds(1)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(1));

            Assert.IsTrue(function.Running);
            Assert.AreEqual(State.Processing, function.State);

            while (function.Running && DateTime.UtcNow - startTime < duration.Add(TimeSpan.FromSeconds(1)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            var totalDuration = DateTime.UtcNow - startTime;

            Assert.IsTrue(totalDuration >= duration.Subtract(TimeSpan.FromMilliseconds(5)) && totalDuration <= duration.Add(TimeSpan.FromMilliseconds(5)));
        }

        [TestMethod]
        public void EndToEnd()
        {
            var start = new DateTime(3);
            var middle = new DateTime(2);
            var end = new DateTime(1);

            var startFunction = new TestFunction(() => start = DateTime.UtcNow);
            var middleFunction = new TestFunction(() => middle = DateTime.UtcNow);
            var endFunction = new TestFunction(() => end = DateTime.UtcNow);

            Assert.IsTrue(end < middle && middle < start);

            var a = startFunction.CreateOutputPublic("A");
            var b = middleFunction.CreateInputPublic("B");
            var c = middleFunction.CreateOutputPublic("C");
            var d = endFunction.CreateInputPublic("D");

            startFunction.OutputToTrigger = a;
            middleFunction.OutputToTrigger = c;

            a.Connect(b);
            c.Connect(d);

            startFunction.Trigger(null);

            var startTime = DateTime.UtcNow;

            while (!endFunction.HasRun && DateTime.UtcNow < startTime.AddSeconds(1))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(start < middle && middle < end);
        }

        [TestMethod]
        public void Completion()
        {
            var startFunction = new TestFunction(() => { });
            var output = startFunction.CreateOutputPublic();

            var endFunction = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var input = endFunction.CreateInputPublic();

            startFunction.OutputToTrigger = output;

            Assert.IsTrue(output.Connect(input));

            startFunction.Trigger(null);

            var startTime = DateTime.UtcNow;

            while ((!startFunction.HasRun || startFunction.Running || startFunction.State != State.Waiting || !endFunction.Running) && DateTime.UtcNow < startTime.AddSeconds(2))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.AreNotEqual(startFunction.HasRun, endFunction.HasRun);
        }
    }
}
