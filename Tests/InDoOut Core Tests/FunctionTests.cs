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

            Assert.IsNull(input);
            Assert.IsNull(output);

            output = function.CreateOutputPublic(OutputType.Negative);

            Assert.IsNull(output);

            output = function.CreateOutputPublic(OutputType.Positive);

            Assert.IsNull(output);

            output = function.CreateOutputPublic(OutputType.Neutral);

            Assert.IsNull(output);

            Assert.AreEqual(1, function.Inputs.Count);
            Assert.AreEqual(1, function.Outputs.Count);

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

        [TestMethod]
        public void PolitelyStop()
        {
            var stoppedSafely = false;
            var count = 0;
            var function = new TestFunction();

            function.Action = () =>
            {
                for (count = 0; count < 100; ++count)
                {
                    if (function.StopRequested)
                    {
                        stoppedSafely = true;
                        return;
                    }

                    Thread.Sleep(TimeSpan.FromMilliseconds(10));
                }
            };

            Assert.IsFalse(stoppedSafely);

            function.Trigger(null);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            Assert.IsTrue(function.Running);
            Assert.IsFalse(function.HasRun);
            Assert.IsFalse(stoppedSafely);

            function.PolitelyStop();

            var startTime = DateTime.UtcNow;

            while (!function.HasRun && DateTime.UtcNow < startTime.AddSeconds(1))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(stoppedSafely);
            Assert.AreNotEqual(100, count);
        }

        [TestMethod]
        public void NoTriggerOnStop()
        {
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var lastFunction = new TestFunction();

            var output = function.CreateOutputPublic();
            var input = function.CreateInputPublic();

            Assert.IsTrue(output.Connect(input));

            function.OutputToTrigger = output;
            function.Trigger(null);

            var startTime = DateTime.UtcNow;

            while (!function.Running && DateTime.UtcNow < startTime.Add(TimeSpan.FromSeconds(1)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(function.Running);

            function.PolitelyStop();

            startTime = DateTime.UtcNow;

            while (function.Running && DateTime.UtcNow < startTime.Add(TimeSpan.FromSeconds(1)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            startTime = DateTime.UtcNow;

            while (!lastFunction.Running && DateTime.UtcNow < startTime.Add(TimeSpan.FromMilliseconds(10)))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsFalse(lastFunction.Running);
            Assert.IsFalse(lastFunction.HasRun);
            Assert.IsTrue(function.HasRun);
        }

        [TestMethod]
        public void DefaultInputOutputTypes()
        {
            var function = new TestFunction();
            var input = function.CreateInputPublic();
            var neutral = function.CreateOutputPublic("Neutral", OutputType.Neutral);
            var positive = function.CreateOutputPublic("Positive", OutputType.Positive);
            var negative = function.CreateOutputPublic("Negative", OutputType.Negative);

            Assert.IsInstanceOfType(input, typeof(Input));
            Assert.IsInstanceOfType(neutral, typeof(OutputNeutral));
            Assert.IsInstanceOfType(positive, typeof(OutputPositive));
            Assert.IsInstanceOfType(negative, typeof(OutputNegative));

            Assert.IsInstanceOfType(input, typeof(IInput));
            Assert.IsInstanceOfType(neutral, typeof(IOutputNeutral));
            Assert.IsInstanceOfType(positive, typeof(IOutputPositive));
            Assert.IsInstanceOfType(negative, typeof(IOutputNegative));

            Assert.IsInstanceOfType(neutral, typeof(IOutput));
            Assert.IsInstanceOfType(positive, typeof(IOutput));
            Assert.IsInstanceOfType(negative, typeof(IOutput));

            Assert.IsNotInstanceOfType(neutral, typeof(IInput));
            Assert.IsNotInstanceOfType(positive, typeof(IInput));
            Assert.IsNotInstanceOfType(negative, typeof(IInput));
            Assert.IsNotInstanceOfType(input, typeof(IOutput));

            Assert.IsNotInstanceOfType(neutral, typeof(IOutputNegative));
            Assert.IsNotInstanceOfType(neutral, typeof(IOutputPositive));

            Assert.IsNotInstanceOfType(positive, typeof(IOutputNegative));
            Assert.IsNotInstanceOfType(positive, typeof(IOutputNeutral));

            Assert.IsNotInstanceOfType(negative, typeof(IOutputPositive));
            Assert.IsNotInstanceOfType(negative, typeof(IOutputNeutral));

            var newInput = function.CreateInputPublic("B");
            var newNeutral = function.CreateOutputPublic("Neutral B", OutputType.Neutral);
            var newPositive = function.CreateOutputPublic("Positive B", OutputType.Positive);
            var newNegative = function.CreateOutputPublic("Negative B", OutputType.Negative);

            Assert.AreNotEqual(input, newInput);
            Assert.AreNotEqual(neutral, newNeutral);
            Assert.AreNotEqual(positive, newPositive);
            Assert.AreNotEqual(negative, newNegative);
        }

        [TestMethod]
        public void CustomInputOutputTypes()
        {
            var function = new TestFunction();
            var input = function.CreateInputPublic("Basic bitch");
            var neutral = function.CreateOutputPublic("Neutral", OutputType.Neutral);
            var positive = function.CreateOutputPublic("Positive", OutputType.Positive);
            var negative = function.CreateOutputPublic("Negative", OutputType.Negative);

            Assert.IsInstanceOfType(input, typeof(Input));
            Assert.IsInstanceOfType(neutral, typeof(OutputNeutral));
            Assert.IsInstanceOfType(positive, typeof(OutputPositive));
            Assert.IsInstanceOfType(negative, typeof(OutputNegative));

            var newInput = new Input(null, "Actually different");
            var newNeutral = new OutputNegative("Actually negative");
            var newPositive = new OutputNeutral("Actually neutral");
            var newNegative = new OutputPositive("Actually positive");

            function.InputToBuild = newInput;
            function.OutputToBuild = newNeutral;

            var outInput = function.CreateInputPublic();

            Assert.AreEqual(newInput, outInput);
            Assert.AreNotEqual(outInput, input);

            var outNeutral = function.CreateOutputPublic();

            Assert.AreEqual(newNeutral, outNeutral);

            outNeutral = function.CreateOutputPublic("Doesn't actually matter anymore");

            Assert.IsNull(outNeutral);

            outNeutral = function.CreateOutputPublic("Just gets ignored now", OutputType.Negative);

            Assert.IsNull(outNeutral);

            function.OutputToBuild = newPositive;

            var outPositive = function.CreateOutputPublic();

            Assert.AreEqual(newPositive, outPositive);

            function.OutputToBuild = newNegative;

            var outNegative = function.CreateOutputPublic();

            Assert.AreEqual(newNegative, outNegative);
        }

        [TestMethod]
        public void ExceptionSafety()
        {
            var function = new ExceptionFunction();

            Assert.ThrowsException<Exception>(() => function.Name);
            Assert.ThrowsException<Exception>(() => function.Description);
            Assert.ThrowsException<Exception>(() => function.Group);
            Assert.ThrowsException<Exception>(() => function.Keywords);

            Assert.IsNull(function.SafeName);
            Assert.IsNull(function.SafeDescription);
            Assert.IsNull(function.SafeGroup);
            Assert.IsNull(function.SafeKeywords);

            function.Trigger(null);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            Assert.AreEqual(State.InError, function.State);
        }
    }
}
