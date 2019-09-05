using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Variables;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class ResultTests
    {
        [TestMethod]
        public void Constructors()
        {
            var result = new Result("name", "description");

            Assert.AreEqual("name", result.Name);
            Assert.AreEqual("description", result.Description);
            Assert.AreEqual("", result.RawValue);

            result = new Result("name b", "description b", "a value");

            Assert.AreEqual("name b", result.Name);
            Assert.AreEqual("description b", result.Description);
            Assert.AreEqual("a value", result.RawValue);
        }

        [TestMethod]
        public void ConnectDisconnect()
        {
            var resultA = new Result("Result A", "", "Value A");
            var resultB = new Result("Result B", "", "Value B");

            var propertyA = new Property<string>("Property A", "", false, "Property A Basic");
            var propertyB = new Property<string>("Property B", "", false, "Property B Basic");

            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromMilliseconds(10))) { VariableStore = new TestVariableStore() };
            _ = function.AddResultPublic(resultA);
            _ = function.AddResultPublic(resultB);
            _ = function.AddPropertyPublic(propertyA);
            _ = function.AddPropertyPublic(propertyB);

            Assert.AreEqual("Property A Basic", propertyA.FullValue);
            Assert.AreEqual("Property B Basic", propertyB.FullValue);

            Assert.IsTrue(propertyA.CanAcceptConnection(resultA));
            Assert.IsTrue(propertyB.CanAcceptConnection(resultA));
            Assert.IsTrue(propertyA.CanAcceptConnection(resultB));
            Assert.IsTrue(propertyB.CanAcceptConnection(resultB));

            Assert.IsTrue(resultA.Connect(propertyA));
            Assert.IsTrue(resultB.Connect(propertyB));

            Assert.AreEqual("Property A Basic", propertyA.FullValue);
            Assert.AreEqual("Property B Basic", propertyB.FullValue);

            resultA.Trigger(function);

            Assert.IsTrue(resultA.WaitForCompletion(TimeSpan.FromMilliseconds(100), false));
            Assert.AreEqual("Value A", propertyA.FullValue);
            Assert.AreEqual("Property B Basic", propertyB.FullValue);

            resultB.Trigger(function);

            Assert.IsTrue(resultB.WaitForCompletion(TimeSpan.FromMilliseconds(100), false));
            Assert.AreEqual("Value A", propertyA.FullValue);
            Assert.AreEqual("Value B", propertyB.FullValue);

            Assert.IsTrue(resultA.Connect(propertyB));

            resultB.Trigger(function);

            Assert.IsTrue(resultB.WaitForCompletion(TimeSpan.FromMilliseconds(100), false));
            Assert.AreEqual("Value A", propertyA.FullValue);
            Assert.AreEqual("Value B", propertyB.FullValue);

            resultA.Trigger(function);

            Assert.IsTrue(resultA.WaitForCompletion(TimeSpan.FromMilliseconds(100), false));
            Assert.AreEqual("Value A", propertyA.FullValue);
            Assert.AreEqual("Value A", propertyB.FullValue);

            resultB.Trigger(function);

            Assert.IsTrue(resultB.WaitForCompletion(TimeSpan.FromMilliseconds(100), false));
            Assert.AreEqual("Value A", propertyA.FullValue);
            Assert.AreEqual("Value B", propertyB.FullValue);

            Assert.IsTrue(resultA.Disconnect(propertyB));

            resultA.Trigger(function);

            Assert.IsTrue(resultA.WaitForCompletion(TimeSpan.FromMilliseconds(100), false));
            Assert.AreEqual("Value A", propertyA.FullValue);
            Assert.AreEqual("Value B", propertyB.FullValue);
        }

        [TestMethod]
        public void SetVariableStore()
        {
            var result = new Result("name", "description", "nothing");
            var variableStore = new TestVariableStore();

            result.VariableName = null;

            Assert.IsFalse(result.SetVariable(variableStore));
            Assert.AreEqual(0, variableStore.PublicVariables.Count);

            result.VariableName = "a variable name";

            Assert.IsTrue(result.SetVariable(variableStore));
            Assert.AreEqual(1, variableStore.PublicVariables.Count);
            Assert.AreEqual("nothing", variableStore.GetVariableValue(result.VariableName));

            result.VariableName = "a different name";
            result.RawValue = "something";

            Assert.IsTrue(result.SetVariable(variableStore));
            Assert.AreEqual(2, variableStore.PublicVariables.Count);
            Assert.AreEqual("something", variableStore.GetVariableValue(result.VariableName));

            result.VariableName = "a variable name";

            Assert.IsTrue(result.SetVariable(variableStore));
            Assert.AreEqual(2, variableStore.PublicVariables.Count);
            Assert.AreEqual("something", variableStore.GetVariableValue(result.VariableName));
        }

        [TestMethod]
        public void SetVariable()
        {
            var result = new Result("name", "description", "nothing");
            var variable = new Variable("variable name");

            result.VariableName = "a different variable name";

            Assert.AreNotEqual(result.RawValue, variable.RawValue);
            Assert.AreNotEqual(result.VariableName, variable.Name);

            _ = result.SetVariable(variable);

            Assert.AreEqual(result.RawValue, variable.RawValue);
            Assert.AreNotEqual(result.VariableName, variable.Name);

            result.RawValue = "something else";

            _ = result.SetVariable(variable);

            Assert.AreEqual(result.RawValue, variable.RawValue);
            Assert.AreNotEqual(result.VariableName, variable.Name);
        }
    }
}
