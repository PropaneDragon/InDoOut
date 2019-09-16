using InDoOut_Core.Entities.Functions;
using InDoOut_Testing;
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
        public void AddProperties()
        {
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(50)));

            Assert.AreEqual(0, function.Properties.Count);

            var property = function.AddPropertyPublic(new Property<double>("name", "description", true, 5432.109));

            Assert.AreEqual(1, function.Properties.Count);
            Assert.IsNotNull(property);
            Assert.AreEqual("name", property.Name);
            Assert.AreEqual("description", property.Description);
            Assert.IsTrue(property.Required);
            Assert.AreEqual(5432.109d, property.BasicValue);

            var comparisonProperty = new Property<string>("another name", "another description", false, "some value");
            var resultProperty = function.AddPropertyPublic(comparisonProperty);

            Assert.AreEqual(2, function.Properties.Count);
            Assert.AreEqual(comparisonProperty, resultProperty);

            var duplicateProperty = function.AddPropertyPublic(comparisonProperty);

            Assert.AreEqual(2, function.Properties.Count);
            Assert.AreEqual(comparisonProperty, duplicateProperty);
        }

        [TestMethod]
        public void AddResults()
        {
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(50)));

            Assert.AreEqual(0, function.Results.Count);

            var result = function.AddResultPublic(new Result("name", "description", "a result"));

            Assert.AreEqual(1, function.Results.Count);
            Assert.IsNotNull(result);
            Assert.AreEqual("name", result.Name);
            Assert.AreEqual("description", result.Description);
            Assert.AreEqual("a result", result.RawValue);

            var comparisonResult = new Result("another name", "another description", "another result value");
            var resultResult = function.AddResultPublic(comparisonResult);

            Assert.AreEqual(2, function.Results.Count);
            Assert.AreEqual(comparisonResult, resultResult);

            var duplicateResult = function.AddResultPublic(comparisonResult);

            Assert.AreEqual(2, function.Results.Count);
            Assert.AreEqual(comparisonResult, duplicateResult);
        }

        [TestMethod]
        public void PropertiesMirroredAsResults()
        {
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(10)));

            Assert.AreEqual(0, function.Properties.Count);
            Assert.AreEqual(0, function.Results.Count);

            Assert.IsNotNull(function.AddPropertyPublic(new Property<string>("Test property", "Test description")));

            Assert.AreEqual(1, function.Properties.Count);
            Assert.AreEqual(1, function.Results.Count);

            Assert.AreEqual("Test property *", function.Results[0].Name);
            Assert.AreEqual("Passthrough: Test description", function.Results[0].Description);

            Assert.IsNotNull(function.AddPropertyPublic(new Property<string>("Test property 2", "Test description 2"), false));

            Assert.AreEqual(2, function.Properties.Count);
            Assert.AreEqual(1, function.Results.Count);

            Assert.AreEqual("Test property *", function.Results[0].Name);
            Assert.AreEqual("Passthrough: Test description", function.Results[0].Description);

            Assert.IsNotNull(function.AddPropertyPublic(new Property<string>("Test property 3", "Test description 3"), true));

            Assert.AreEqual(3, function.Properties.Count);
            Assert.AreEqual(2, function.Results.Count);

            Assert.AreEqual("Test property *", function.Results[0].Name);
            Assert.AreEqual("Passthrough: Test description", function.Results[0].Description);

            Assert.AreEqual("Test property 3 *", function.Results[1].Name);
            Assert.AreEqual("Passthrough: Test description 3", function.Results[1].Description);
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


            _ = outputA.Connect(inputA);
            _ = outputB.Connect(inputB);

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
            CheckDuration(TimeSpan.FromMilliseconds(3));
            CheckDuration(TimeSpan.FromMilliseconds(50));
            CheckDuration(TimeSpan.FromMilliseconds(5));
            CheckDuration(TimeSpan.FromMilliseconds(16.2));
            CheckDuration(TimeSpan.FromMilliseconds(200));
            CheckDuration(TimeSpan.FromMilliseconds(11));
        }

        private void CheckDuration(TimeSpan duration)
        {
            var function = new TestFunction(() => Thread.Sleep(duration));

            function.Trigger(null);

            var startTime = DateTime.UtcNow;

            Assert.IsTrue(function.WaitForCompletion(duration.Add(TimeSpan.FromSeconds(1)), true));

            var totalDuration = DateTime.UtcNow - startTime;

            Assert.AreEqual(duration.TotalMilliseconds, totalDuration.TotalMilliseconds, 5);
        }

        [TestMethod]
        public void EndToEnd()
        {
            var start = new DateTime(3);
            var middle = new DateTime(2);
            var end = new DateTime(1);

            var variableStore = new TestVariableStore();

            var startFunction = new TestFunction(() => start = DateTime.UtcNow) { VariableStore = variableStore };
            var middleFunction = new TestFunction(() => middle = DateTime.UtcNow) { VariableStore = variableStore };
            var endFunction = new TestFunction(() => end = DateTime.UtcNow) { VariableStore = variableStore };

            Assert.IsTrue(end < middle && middle < start);

            var a = startFunction.CreateOutputPublic("A");
            var b = middleFunction.CreateInputPublic("B");
            var c = middleFunction.CreateOutputPublic("C");
            var d = endFunction.CreateInputPublic("D");

            var e = startFunction.AddResultPublic(new Result("result", "Nope", "100"));
            var f = endFunction.AddPropertyPublic(new Property<int>("property", "Nope", false, 0));

            var variableName = e.VariableName;

            startFunction.OutputToTrigger = a;
            middleFunction.OutputToTrigger = c;

            Assert.IsTrue(a.Connect(b));
            Assert.IsTrue(c.Connect(d));

            Assert.IsTrue(e.Connect(f));

            startFunction.Trigger(null);

            var startTime = DateTime.UtcNow;

            while (!endFunction.HasRun && DateTime.UtcNow < startTime.AddSeconds(1))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(start < middle && middle < end);
            Assert.AreEqual(100, f.FullValue);
            Assert.IsNotNull(variableStore.GetVariable(variableName));
            Assert.AreEqual("100", variableStore.GetVariableValue(variableName));
        }

        [TestMethod]
        public void InputToOutputWithProperties()
        {
            var variableStore = new TestVariableStore();
            var fullFunction = new TestFullFunction
            {
                VariableStore = variableStore
            };

            Assert.AreEqual("", fullFunction.IntegerResult.RawValue);
            Assert.AreEqual("", fullFunction.DoubleResult.RawValue);
            Assert.AreEqual("", fullFunction.FloatResult.RawValue);
            Assert.AreEqual("", fullFunction.StringResult.RawValue);
            Assert.AreEqual(0, variableStore.PublicVariables.Count);

            fullFunction.IntegerProperty.BasicValue = 1234;
            fullFunction.DoubleProperty.BasicValue = 456.78901d;
            fullFunction.FloatProperty.BasicValue = 789.01f;
            fullFunction.StringProperty.BasicValue = null;

            fullFunction.IntegerResult.VariableName = "Int";
            fullFunction.DoubleResult.VariableName = "Double";
            fullFunction.FloatResult.VariableName = "Float";
            fullFunction.StringResult.VariableName = "String";

            Assert.AreEqual("", fullFunction.IntegerResult.RawValue);
            Assert.AreEqual("", fullFunction.DoubleResult.RawValue);
            Assert.AreEqual("", fullFunction.FloatResult.RawValue);
            Assert.AreEqual("", fullFunction.StringResult.RawValue);
            Assert.AreEqual(0, variableStore.PublicVariables.Count);

            fullFunction.Trigger(null);

            Assert.IsTrue(fullFunction.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.IntegerResult.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.DoubleResult.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.FloatResult.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.StringResult.WaitForCompletion(TimeSpan.FromSeconds(1)));

            Assert.AreEqual("", fullFunction.IntegerResult.RawValue);
            Assert.AreEqual("", fullFunction.DoubleResult.RawValue);
            Assert.AreEqual("", fullFunction.FloatResult.RawValue);
            Assert.AreEqual("", fullFunction.StringResult.RawValue);

            Assert.AreEqual(7, variableStore.PublicVariables.Count);
            Assert.AreEqual("", variableStore.GetVariableValue("Int"));
            Assert.AreEqual("", variableStore.GetVariableValue("Double"));
            Assert.AreEqual("", variableStore.GetVariableValue("Float"));
            Assert.AreEqual("", variableStore.GetVariableValue("String"));

            fullFunction.StringProperty.BasicValue = "A non-null string";
            fullFunction.Trigger(null);

            Assert.IsTrue(fullFunction.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.IntegerResult.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.DoubleResult.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.FloatResult.WaitForCompletion(TimeSpan.FromSeconds(1)));
            Assert.IsTrue(fullFunction.StringResult.WaitForCompletion(TimeSpan.FromSeconds(1)));

            Assert.AreEqual("1234", fullFunction.IntegerResult.RawValue);
            Assert.AreEqual("456.78901", fullFunction.DoubleResult.RawValue);
            Assert.AreEqual("789.01", fullFunction.FloatResult.RawValue);
            Assert.AreEqual("A non-null string", fullFunction.StringResult.RawValue);

            Assert.AreEqual(8, variableStore.PublicVariables.Count);
            Assert.AreEqual("1234", variableStore.GetVariableValue("Int"));
            Assert.AreEqual("456.78901", variableStore.GetVariableValue("Double"));
            Assert.AreEqual("789.01", variableStore.GetVariableValue("Float"));
            Assert.AreEqual("A non-null string", variableStore.GetVariableValue("String"));
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


            _ = Assert.ThrowsException<Exception>(() => function.Name);
            _ = Assert.ThrowsException<Exception>(() => function.Description);
            _ = Assert.ThrowsException<Exception>(() => function.Group);
            _ = Assert.ThrowsException<Exception>(() => function.Keywords);

            Assert.IsNull(function.SafeName);
            Assert.IsNull(function.SafeDescription);
            Assert.IsNull(function.SafeGroup);
            Assert.IsNull(function.SafeKeywords);

            function.Trigger(null);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            Assert.AreEqual(State.InError, function.State);
        }

        [TestMethod]
        public void ResultsToVariables()
        {
            var store = new TestVariableStore();
            var function = new TestFunction()
            {
                VariableStore = store
            };
            var result = function.AddResultPublic(new Result("A result", "A description", "variable value!"));

            result.VariableName = "A set variable";

            Assert.AreEqual("variable value!", result.RawValue);
            Assert.AreNotEqual("variable value!", store.GetVariableValue("A set variable"));

            function.Trigger(null);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            Assert.AreEqual("variable value!", store.GetVariableValue("A set variable"));
        }
    }
}