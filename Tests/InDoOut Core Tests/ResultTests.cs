using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        public void SetVariableStore()
        {
            var result = new Result("name", "description", "nothing");
            var variableStore = new TestVariableStore();

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
