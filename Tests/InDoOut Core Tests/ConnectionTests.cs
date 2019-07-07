using InDoOut_Core.Entities.Functions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class ConnectionTests
    {
        [TestMethod]
        public void Input()
        {
            var input = new Input(null);
            var output = new Output();
            var function = new TestFunction(() => { });

            Assert.IsTrue(input.CanAcceptConnection(output));
            Assert.IsFalse(input.CanAcceptConnection(input));
            Assert.IsFalse(input.CanAcceptConnection(function));
            Assert.IsFalse(input.CanAcceptConnection(null));
        }

        [TestMethod]
        public void Output()
        {
            var input = new Input(null);
            var output = new Output();
            var function = new TestFunction(() => { });

            Assert.IsTrue(output.CanAcceptConnection(function));
            Assert.IsFalse(output.CanAcceptConnection(input));
            Assert.IsFalse(output.CanAcceptConnection(output));
            Assert.IsFalse(output.CanAcceptConnection(null));
        }

        [TestMethod]
        public void Function()
        {
            var input = new Input(null);
            var output = new Output();
            var function = new TestFunction(() => { });

            Assert.IsTrue(function.CanAcceptConnection(input));
            Assert.IsFalse(function.CanAcceptConnection(function));
            Assert.IsFalse(function.CanAcceptConnection(output));
            Assert.IsFalse(function.CanAcceptConnection(null));
        }
    }
}
