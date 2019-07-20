using InDoOut_Core.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class VariableStoreTests
    {
        [TestMethod]
        public void SetGet()
        {
            var variableStore = new TestVariableStore();

            Assert.AreEqual(0, variableStore.PublicVariables.Count);
            Assert.IsNull(variableStore.GetVariableValue("variable a"));

            Assert.IsTrue(variableStore.SetVariable("variable a", "value"));

            Assert.AreEqual(1, variableStore.PublicVariables.Count);
            Assert.AreEqual("value", variableStore.GetVariableValue("vArIaBle a"));

            Assert.IsNull(variableStore.GetVariableValue("variablea"));
            Assert.IsNull(variableStore.GetVariableValue("variable  a"));
            Assert.IsNull(variableStore.GetVariableValue("var1able a"));
            Assert.IsNull(variableStore.GetVariableValue("variable b"));

            var variable = variableStore.GetVariable("variable a");
            Assert.AreEqual("variable a", variable.Name);
            Assert.AreEqual("value", variable.RawValue);

            Assert.IsTrue(variableStore.SetVariable("variable a", "another value"));
            Assert.AreEqual(1, variableStore.PublicVariables.Count);
            Assert.AreEqual("another value", variableStore.GetVariableValue("variable a"));

            Assert.IsTrue(variableStore.SetVariable("variable b", "b value"));
            Assert.AreEqual(2, variableStore.PublicVariables.Count);
            Assert.AreEqual("another value", variableStore.GetVariableValue("variable a"));
            Assert.AreEqual("b value", variableStore.GetVariableValue("variable b"));
            Assert.AreEqual("b value", variableStore.GetVariableValue("VaRiAbLe B"));

            Assert.IsTrue(variableStore.SetVariable(new Variable("variable b", "changed value b")));
            Assert.AreEqual(2, variableStore.PublicVariables.Count);
            Assert.AreEqual("changed value b", variableStore.GetVariableValue("variable b"));

            Assert.IsTrue(variableStore.SetVariable(new Variable("variable c", "value")));
            Assert.AreEqual(3, variableStore.PublicVariables.Count);
            Assert.AreEqual("value", variableStore.GetVariableValue("variable c"));
        }

        [TestMethod]
        public void Conversion()
        {
            var variableStore = new TestVariableStore();

            Assert.AreEqual(0, variableStore.PublicVariables.Count);
            Assert.IsTrue(variableStore.SetVariable("integer", "12"));
            Assert.IsTrue(variableStore.SetVariable("floatdouble", "987.6543"));
            Assert.IsTrue(variableStore.SetVariable("string", "haha"));
            Assert.IsTrue(variableStore.SetVariable("bool", "true"));

            Assert.AreEqual(12, variableStore.GetVariableValueAs<int>("integer"));
            Assert.AreEqual(12, variableStore.GetVariableValueAs<int>("integer", 34));
            Assert.AreEqual(987.6543f, variableStore.GetVariableValueAs<float>("floatdouble"));
            Assert.AreEqual(987.6543f, variableStore.GetVariableValueAs<float>("floatdouble", 84646.5416f));
            Assert.AreEqual(987.6543d, variableStore.GetVariableValueAs<double>("floatdouble"));
            Assert.AreEqual(987.6543d, variableStore.GetVariableValueAs<double>("floatdouble", 541615.515d));
            Assert.IsTrue(variableStore.GetVariableValueAs<bool>("bool"));
            Assert.IsTrue(variableStore.GetVariableValueAs<bool>("bool", false));

            Assert.AreEqual(default, variableStore.GetVariableValueAs<int>("string"));
            Assert.AreEqual(default, variableStore.GetVariableValueAs<bool>("string"));
            Assert.AreEqual(default, variableStore.GetVariableValueAs<double>("string"));
            Assert.AreEqual(default, variableStore.GetVariableValueAs<float>("string"));

            Assert.AreEqual(666, variableStore.GetVariableValueAs("string", 666));
            Assert.AreEqual(false, variableStore.GetVariableValueAs("string", false));
            Assert.AreEqual(876543.32d, variableStore.GetVariableValueAs("string", 876543.32d));
            Assert.AreEqual(1132165.51f, variableStore.GetVariableValueAs("string", 1132165.51f));
        }

        [TestMethod]
        public void Leakage()
        {
            var variableStoreA = new TestVariableStore();
            var variableStoreB = new TestVariableStore();

            Assert.AreEqual(0, variableStoreA.PublicVariables.Count);
            Assert.AreEqual(0, variableStoreB.PublicVariables.Count);

            Assert.IsTrue(variableStoreA.SetVariable("A", "value for A in A"));

            Assert.AreEqual(1, variableStoreA.PublicVariables.Count);
            Assert.AreEqual(0, variableStoreB.PublicVariables.Count);

            Assert.IsTrue(variableStoreB.SetVariable("A", "value for A in B"));

            Assert.AreEqual(1, variableStoreA.PublicVariables.Count);
            Assert.AreEqual(1, variableStoreB.PublicVariables.Count);

            Assert.AreEqual("value for A in A", variableStoreA.GetVariableValue("A"));
            Assert.AreEqual("value for A in B", variableStoreB.GetVariableValue("A"));
        }
    }
}
