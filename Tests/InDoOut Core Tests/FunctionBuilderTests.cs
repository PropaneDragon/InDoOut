using InDoOut_Core.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class FunctionBuilderTests
    {
        [TestMethod]
        public void Instancing()
        {
            var functionBuilder = new FunctionBuilder();
            var instanceFromType = functionBuilder.BuildInstance(typeof(TestFunction));
            var instanceFromGeneric = functionBuilder.BuildInstance<TestFunction>();
            var instanceFromStartFunction = functionBuilder.BuildInstance<TestStartFunction>();

            Assert.IsNotNull(instanceFromType);
            Assert.AreEqual(typeof(TestFunction), instanceFromType.GetType());

            Assert.IsNotNull(instanceFromGeneric);
            Assert.AreEqual(typeof(TestFunction), instanceFromGeneric.GetType());

            Assert.IsNotNull(instanceFromStartFunction);
            Assert.AreEqual(typeof(TestStartFunction), instanceFromStartFunction.GetType());

            Assert.IsNull(functionBuilder.BuildInstance(null));
        }
    }
}
