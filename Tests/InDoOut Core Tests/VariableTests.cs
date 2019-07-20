using InDoOut_Core.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class VariableTests
    {
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
    }
}
