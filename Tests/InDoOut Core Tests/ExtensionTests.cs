using InDoOut_Core.Extensions.String;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void Pluralise()
        {
            Assert.AreEqual("Positive non pluralised number", "Positive non pluralised number".Pluralise(1));
            Assert.AreEqual("Negative non pluralised number", "Negative non pluralised number".Pluralise(-1));
            Assert.AreEqual("Positive pluralised numbers", "Positive pluralised number".Pluralise(2));
            Assert.AreEqual("Positive pluralised numbers", "Positive pluralised number".Pluralise(10));
            Assert.AreEqual("Positive pluralised numbers", "Positive pluralised number".Pluralise(100));
            Assert.AreEqual("Negative pluralised numbers", "Negative pluralised number".Pluralise(-10));
            Assert.AreEqual("A bit longer tests", "A bit longer test".Pluralise(5));
            Assert.AreEqual("ssss", "sss".Pluralise(5131));
        }
    }
}
