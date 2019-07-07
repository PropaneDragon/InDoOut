using InDoOut_Core.Entities.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class InputTests
    {
        [TestMethod]
        public void Defaults()
        {
            var input = new Input(null);

            Assert.AreEqual("Input", input.Name);
            Assert.AreEqual(0, input.Connections.Count);

            var function = new TestFunction(() => { });
            var input2 = new Input(function);

            Assert.AreEqual("Input", input2.Name);
            Assert.AreEqual(1, input2.Connections.Count);
            Assert.AreEqual(function, input2.Parent);
        }

        [TestMethod]
        public void Equality()
        {
            var inputA = new Input(null);
            var inputB = new Input(null);
            var inputC = new Input(null, "Something else");

            Assert.AreEqual(inputA, inputB);
            Assert.AreNotEqual(inputA, inputC);
            Assert.AreNotEqual(inputB, inputC);

            var methodA = new TestFunction(() => { });
            var methodB = new TestFunction(() => { });

            var inputD = new Input(methodA);
            var inputE = new Input(methodB);
            var inputF = new Input(methodA, "Another name");
            var inputG = new Input(methodA);

            Assert.AreEqual(inputD, inputG);
            Assert.AreNotEqual(inputD, inputE);
            Assert.AreNotEqual(inputD, inputF);
        }
    }
}
