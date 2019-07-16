using InDoOut_Core.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void Set()
        {
            var variable = new Variable("a");

            Assert.AreEqual("a", variable.Name);
            Assert.AreEqual("", variable.Value);
            Assert.IsTrue(variable.Valid);

            variable.Value = null;

            Assert.AreEqual("a", variable.Name);
            Assert.IsNull(variable.Value);
            Assert.IsFalse(variable.Valid);

            variable.Value = "Something else";

            Assert.AreEqual("a", variable.Name);
            Assert.AreEqual("Something else", variable.Value);
            Assert.IsTrue(variable.Valid);
        }

        [TestMethod]
        public void Comparison()
        {
            var variableComparisonA = new Variable("comparison A", "test");
            var variableComparisonB = new Variable("CoMpArIsOn a", "another test");
            var variableComparisonC = new Variable("cmparison a", "test");
            var variableComparisonEqualA = new Variable("CompaRison a", "test");

            Assert.IsTrue(variableComparisonA.Equals(variableComparisonB));
            Assert.IsFalse(variableComparisonA.Equals(variableComparisonB, true));
            Assert.IsFalse(variableComparisonA.Equals(variableComparisonC));
            Assert.IsTrue(variableComparisonA.Equals(variableComparisonEqualA));
            Assert.IsTrue(variableComparisonA.Equals(variableComparisonEqualA, true));
        }

        [TestMethod]
        public void Get()
        {
            var variable = new Variable("test", null);

            Assert.IsNull(variable.Value);
            Assert.AreEqual("", variable.ValueOrDefault());
            Assert.AreEqual("not null", variable.ValueOrDefault("not null"));

            variable.Value = "Actually not null";

            Assert.AreEqual("Actually not null", variable.Value);
            Assert.AreEqual("Actually not null", variable.ValueOrDefault());
            Assert.AreEqual("Actually not null", variable.ValueOrDefault("not null"));

            Assert.AreEqual(default, variable.ValueAs<int>());
            Assert.AreEqual(-80085, variable.ValueAs<int>(-80085));

            Assert.AreEqual(default, variable.ValueAs<double>());
            Assert.AreEqual(123.4546d, variable.ValueAs<double>(123.4546));

            Assert.AreEqual(default, variable.ValueAs<float>());
            Assert.AreEqual(-65654.51f, variable.ValueAs<float>(-65654.51f));

            Assert.AreEqual(default, variable.ValueAs<bool>());
            Assert.AreEqual(true, variable.ValueAs<bool>(true));

            variable.Value = "-1234";

            Assert.AreEqual(-1234, variable.ValueAs<int>());
            Assert.AreEqual(-1234, variable.ValueAs<int>(-80085));

            Assert.AreEqual(-1234d, variable.ValueAs<double>());
            Assert.AreEqual(-1234d, variable.ValueAs<double>(123.4546));

            Assert.AreEqual(-1234f, variable.ValueAs<float>());
            Assert.AreEqual(-1234f, variable.ValueAs<float>(-65654.5156f));

            Assert.AreEqual(default, variable.ValueAs<bool>());
            Assert.AreEqual(true, variable.ValueAs<bool>(true));

            variable.Value = "5678.92";

            Assert.AreEqual(default, variable.ValueAs<int>());
            Assert.AreEqual(-80085, variable.ValueAs<int>(-80085));

            Assert.AreEqual(5678.92d, variable.ValueAs<double>());
            Assert.AreEqual(5678.92d, variable.ValueAs<double>(123.4546));

            Assert.AreEqual(5678.92f, variable.ValueAs<float>());
            Assert.AreEqual(5678.92f, variable.ValueAs<float>(-65654.5156f));

            Assert.AreEqual(default, variable.ValueAs<bool>());
            Assert.AreEqual(true, variable.ValueAs<bool>(true));

            variable.Value = "true";

            Assert.AreEqual(default, variable.ValueAs<int>());
            Assert.AreEqual(-80085, variable.ValueAs<int>(-80085));

            Assert.AreEqual(default, variable.ValueAs<double>());
            Assert.AreEqual(123.4546d, variable.ValueAs<double>(123.4546));

            Assert.AreEqual(default, variable.ValueAs<float>());
            Assert.AreEqual(-65654.51f, variable.ValueAs<float>(-65654.51f));

            Assert.AreEqual(true, variable.ValueAs<bool>());
            Assert.AreEqual(true, variable.ValueAs<bool>(true));

            variable.Value = "1";

            Assert.AreEqual(1, variable.ValueAs<int>());
            Assert.AreEqual(1, variable.ValueAs<int>(-80085));

            Assert.AreEqual(1d, variable.ValueAs<double>());
            Assert.AreEqual(1d, variable.ValueAs<double>(123.4546));

            Assert.AreEqual(1f, variable.ValueAs<float>());
            Assert.AreEqual(1f, variable.ValueAs<float>(-65654.51f));

            Assert.AreEqual(default, variable.ValueAs<bool>());
            Assert.AreEqual(true, variable.ValueAs<bool>(true));
        }
    }
}
