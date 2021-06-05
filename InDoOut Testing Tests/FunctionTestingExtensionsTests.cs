using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Logging;
using InDoOut_Core_Tests;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace InDoOut_Testing_Tests
{
    [TestClass]
    public class FunctionTestingExtensionsTests
    {
        [TestMethod]
        public void WaitForCompletion()
        {
            Log.Instance.Enabled = false;

            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(50)));
            function.Trigger(null);

            while (!function.Running)
            {
                Thread.Sleep(1);
            }

            var stopwatch = Stopwatch.StartNew();

            Assert.IsTrue(function.WaitForCompletion(TimeSpan.FromMilliseconds(100)));

            var waitTime = stopwatch.Elapsed;

            Assert.AreEqual(50, waitTime.TotalMilliseconds, 50, $"Total time: {waitTime.TotalMilliseconds}ms");

            function = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(2)));
            function.Trigger(null);

            stopwatch.Restart();

            Assert.IsFalse(function.WaitForCompletion(TimeSpan.FromMilliseconds(60)));
            
            waitTime = stopwatch.Elapsed;

            Assert.AreEqual(60, waitTime.TotalMilliseconds, 50, $"Total time: {waitTime.TotalMilliseconds}ms");

            function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(10)));
            function.Trigger(null);

            stopwatch.Restart();

            function.WaitForCompletion();

            waitTime = stopwatch.Elapsed;

            Assert.AreEqual(10, waitTime.TotalMilliseconds, 50, $"Total time: {waitTime.TotalMilliseconds}ms");

            Log.Instance.Enabled = true;
        }

        [TestMethod]
        public void WaitForStartAndCompletion()
        {
            Log.Instance.Enabled = false;

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

            Assert.AreEqual(100, waitTime.TotalMilliseconds, 100, $"Total time: {waitTime.TotalMilliseconds}");

            Log.Instance.Enabled = true;
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

        [TestMethod]
        public void GetResultValue()
        {
            var function = new TestFunction();
            var resultA = function.AddResultPublic(new Result("result A", "", "result A value"));
            var resultB = function.AddResultPublic(new Result("result B", "", "result B value"));
            var resultC = function.AddResultPublic(new Result("result C", "", "result C value"));

            Assert.IsNotNull(resultA);
            Assert.IsNotNull(resultB);
            Assert.IsNotNull(resultC);

            Assert.AreEqual("result A value", function.GetResultValue("result A"));
            Assert.AreEqual("result B value", function.GetResultValue("result B"));
            Assert.AreEqual("result C value", function.GetResultValue("result C"));

            resultA.RawValue = "another value A";
            resultB.RawValue = "another value B";
            resultC.RawValue = "another value C";

            Assert.AreEqual("another value A", function.GetResultValue("result A"));
            Assert.AreEqual("another value B", function.GetResultValue("result B"));
            Assert.AreEqual("another value C", function.GetResultValue("result C"));

            Assert.IsNull(function.GetResultValue("not a result name"));
        }

        [TestMethod]
        public void Getters()
        {
            var function = new TestFunction();
            var defaultInput = function.CreateInputPublic();
            var input = function.CreateInputPublic("input name");
            var defaultOutput = function.CreateOutputPublic();
            var output = function.CreateOutputPublic("output name");
            var property1 = function.AddPropertyPublic(new Property<string>("property name", ""));
            var property2 = function.AddPropertyPublic(new Property<string>("another property name", ""));
            var result1 = function.AddResultPublic(new Result("result name", ""));
            var result2 = function.AddResultPublic(new Result("another result name", ""));

            Assert.AreEqual(defaultInput, function.GetInputByName("Input"));
            Assert.AreEqual(input, function.GetInputByName("input name"));
            Assert.IsNull(function.GetInputByName("a name that doesn't exist"));
            Assert.IsNull(function.GetInputByName(""));
            Assert.IsNull(function.GetInputByName(null));

            Assert.AreEqual(defaultOutput, function.GetOutputByName("Output"));
            Assert.AreEqual(output, function.GetOutputByName("output name"));
            Assert.IsNull(function.GetOutputByName("a name that doesn't exist"));
            Assert.IsNull(function.GetOutputByName(""));
            Assert.IsNull(function.GetOutputByName(null));

            Assert.AreEqual(property1, function.GetPropertyByName("property name"));
            Assert.AreEqual(property2, function.GetPropertyByName("another property name"));
            Assert.IsNull(function.GetPropertyByName("a name that doesn't exist"));
            Assert.IsNull(function.GetPropertyByName(""));
            Assert.IsNull(function.GetPropertyByName(null));

            Assert.AreEqual(result1, function.GetResultByName("result name"));
            Assert.AreEqual(result2, function.GetResultByName("another result name"));
            Assert.IsNull(function.GetResultByName("a name that doesn't exist"));
            Assert.IsNull(function.GetResultByName(""));
            Assert.IsNull(function.GetResultByName(null));
        }
    }
}