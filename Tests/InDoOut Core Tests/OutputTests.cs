﻿using InDoOut_Core.Entities.Functions;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class OutputTests
    {
        [TestMethod]
        public void SingleConnection()
        {
            var output = new OutputNeutral();
            var functionA = new TestFunction(() => { });

            var a = functionA.CreateInputPublic();

            Assert.IsFalse(functionA.HasRun);
            Assert.IsNull(functionA.LastInput);
            Assert.IsTrue(output.Connect(a));

            output.Trigger(null);

            var startTime = DateTime.UtcNow;

            while (!functionA.HasRun && DateTime.UtcNow < startTime.AddSeconds(1))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(functionA.HasRun);
            Assert.AreEqual(a, functionA.LastInput);
        }

        [TestMethod]
        public void Disconnection()
        {
            var output = new OutputNeutral();
            var functionA = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(10)));

            var a = functionA.CreateInputPublic();

            Assert.IsFalse(functionA.HasRun);
            Assert.IsNull(functionA.LastInput);
            Assert.IsTrue(output.Connect(a));
            Assert.AreEqual(1, output.Connections.Count);

            output.Trigger(null);

            Assert.IsTrue(functionA.WaitForCompletion(TimeSpan.FromMilliseconds(500), true));
            Assert.IsTrue(functionA.HasRun);
            Assert.AreEqual(a, functionA.LastInput);

            Assert.IsTrue(output.Disconnect(a));
            Assert.AreEqual(0, output.Connections.Count);

            output.Trigger(null);

            Assert.IsFalse(functionA.WaitForCompletion(TimeSpan.FromMilliseconds(500), true));
        }

        [TestMethod]
        public void MultiConnection()
        {
            var output = new OutputNeutral();
            var functionA = new TestFunction(() => { });
            var functionB = new TestFunction(() => { });
            var functionC = new TestFunction(() => { });
            var functionD = new TestFunction(() => { });

            var a = functionA.CreateInputPublic();
            var b = functionB.CreateInputPublic();
            var c = functionC.CreateInputPublic();
            var d = functionD.CreateInputPublic();

            Assert.IsFalse(functionA.HasRun);
            Assert.IsFalse(functionB.HasRun);
            Assert.IsFalse(functionC.HasRun);
            Assert.IsFalse(functionD.HasRun);

            Assert.IsNull(functionA.LastInput);
            Assert.IsNull(functionB.LastInput);
            Assert.IsNull(functionC.LastInput);
            Assert.IsNull(functionD.LastInput);

            Assert.IsTrue(output.Connect(a));
            Assert.IsTrue(output.Connect(b));
            Assert.IsTrue(output.Connect(c));
            Assert.IsTrue(output.Connect(d));

            output.Trigger(null);

            var startTime = DateTime.UtcNow;

            while ((!functionA.HasRun || !functionB.HasRun || !functionC.HasRun || !functionD.HasRun) && DateTime.UtcNow < startTime.AddSeconds(1))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(functionA.HasRun);
            Assert.AreEqual(a, functionA.LastInput);
            Assert.IsTrue(functionB.HasRun);
            Assert.AreEqual(b, functionB.LastInput);
            Assert.IsTrue(functionC.HasRun);
            Assert.AreEqual(c, functionC.LastInput);
            Assert.IsTrue(functionD.HasRun);
            Assert.AreEqual(d, functionD.LastInput);
        }

        [TestMethod]
        public void MultiConnectionThreading()
        {
            var output = new OutputNeutral();
            var functionA = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var functionB = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var functionC = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var functionD = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));

            var a = functionA.CreateInputPublic();
            var b = functionB.CreateInputPublic();
            var c = functionC.CreateInputPublic();
            var d = functionD.CreateInputPublic();

            Assert.IsTrue(output.Connect(a));
            Assert.IsTrue(output.Connect(b));
            Assert.IsTrue(output.Connect(c));
            Assert.IsTrue(output.Connect(d));

            output.Trigger(null);

            var startTime = DateTime.UtcNow;

            while ((!functionA.HasRun || !functionB.HasRun || !functionC.HasRun || !functionD.HasRun) && DateTime.UtcNow < startTime.AddSeconds(2))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(functionA.HasRun);
            Assert.AreEqual(a, functionA.LastInput);
            Assert.IsTrue(functionB.HasRun);
            Assert.AreEqual(b, functionB.LastInput);
            Assert.IsTrue(functionC.HasRun);
            Assert.AreEqual(c, functionC.LastInput);
            Assert.IsTrue(functionD.HasRun);
            Assert.AreEqual(d, functionD.LastInput);
        }

        [TestMethod]
        public void Defaults()
        {
            var output = new OutputNeutral();

            Assert.AreEqual("Output", output.Name);
            Assert.AreEqual(0, output.Connections.Count);
        }

        [TestMethod]
        public void Equality()
        {
            var outputA = new OutputNeutral();
            var outputB = new OutputNeutral();
            var outputC = new OutputNeutral("Different name");

            Assert.AreEqual(outputA, outputB);
            Assert.AreNotEqual(outputA, outputC);

            _ = outputA.Connect(new Input(null));

            Assert.AreNotEqual(outputA, outputB);
            Assert.AreNotEqual(outputA, outputC);
        }
    }
}
