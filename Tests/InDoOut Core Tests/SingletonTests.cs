using InDoOut_Core.Instancing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    class TestSingleton : Singleton<TestSingleton>
    {
        public int UniqueNumber { get; set; } = -1;
    }

    [TestClass]
    public class SingletonTests
    {
        [TestMethod]
        public void Instancing()
        {
            Assert.IsNotNull(TestSingleton.Instance);

            var instanceA = TestSingleton.Instance;
            var instanceB = TestSingleton.Instance;
            var instanceC = new TestSingleton();

            Assert.AreEqual(instanceA, instanceB);
            Assert.AreEqual(TestSingleton.Instance, instanceA);
            Assert.AreEqual(TestSingleton.Instance, instanceB);
            Assert.AreNotEqual(instanceA, instanceC);
            Assert.AreNotEqual(instanceB, instanceC);

            instanceC.UniqueNumber = 2;

            Assert.AreEqual(2, instanceC.UniqueNumber);
            Assert.AreNotEqual(2, instanceA.UniqueNumber);
            Assert.AreNotEqual(2, instanceB.UniqueNumber);

            TestSingleton.Instance.UniqueNumber = 5;

            Assert.AreEqual(2, instanceC.UniqueNumber);
            Assert.AreEqual(5, instanceA.UniqueNumber);
            Assert.AreEqual(5, instanceB.UniqueNumber);
            Assert.AreEqual(5, TestSingleton.Instance.UniqueNumber);
        }
    }
}
