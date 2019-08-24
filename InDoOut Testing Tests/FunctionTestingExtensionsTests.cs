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

            function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(10)));
            function.Trigger(null);

            startTime = DateTime.UtcNow;

            function.WaitForCompletion();

            waitTime = DateTime.UtcNow - startTime;

            Assert.AreEqual(10, waitTime.TotalMilliseconds, 10, $"Total time: {waitTime.TotalMilliseconds}ms");
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

            Assert.IsTrue(endFunction.WaitForCompletion(TimeSpan.FromSeconds(1), true));

            var waitTime = DateTime.UtcNow - startTime;

            Assert.AreEqual(100, waitTime.TotalMilliseconds, 10, $"Total time: {waitTime.TotalMilliseconds}");
        }

        [TestMethod]
        public void SetPropertyValue()
        {
            var function = new TestFunction();
            var propertyA = function.AddPropertyPublic(new Property<int>("name a", "", false, 0));
            var propertyB = function.AddPropertyPublic(new Property<string>("name b", "", false, "default"));
            var propertyC = function.AddPropertyPublic(new Property<bool>("name c", "", false, false));

            Assert.IsNotNull(propertyA);
            Assert.IsNotNull(propertyB);
            Assert.IsNotNull(propertyC);

            Assert.AreEqual("0", propertyA.RawValue);
            Assert.AreEqual("default", propertyB.RawValue);
            Assert.AreEqual("False", propertyC.RawValue);

            Assert.IsFalse(function.SetPropertyValue("not valid", "nah"));
            Assert.IsFalse(function.SetPropertyValue("", "nah"));
            Assert.IsFalse(function.SetPropertyValue(null, "nah"));
            Assert.IsFalse(function.SetPropertyValue("name", "nah"));

            Assert.AreEqual("0", propertyA.RawValue);
            Assert.AreEqual("default", propertyB.RawValue);
            Assert.AreEqual("False", propertyC.RawValue);

            Assert.IsTrue(function.SetPropertyValue("name a", "85"));

            Assert.AreEqual("85", propertyA.RawValue);
            Assert.AreEqual("default", propertyB.RawValue);
            Assert.AreEqual("False", propertyC.RawValue);

            Assert.IsTrue(function.SetPropertyValue("name b", "not default any more"));

            Assert.AreEqual("85", propertyA.RawValue);
            Assert.AreEqual("not default any more", propertyB.RawValue);
            Assert.AreEqual("False", propertyC.RawValue);

            Assert.IsTrue(function.SetPropertyValue("name c", "True"));

            Assert.AreEqual("85", propertyA.RawValue);
            Assert.AreEqual("not default any more", propertyB.RawValue);
            Assert.AreEqual("True", propertyC.RawValue);
        }
    }
}
