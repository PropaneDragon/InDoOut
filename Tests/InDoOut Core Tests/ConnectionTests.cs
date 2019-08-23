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
            var property = new Property<int>("", "");
            var function = new TestFunction(() => { });
            var result = new Result("", "");
            var output = new OutputNeutral();

            Assert.IsTrue(input.CanAcceptConnection(output));
            Assert.IsFalse(input.CanAcceptConnection(input));
            Assert.IsFalse(input.CanAcceptConnection(property));
            Assert.IsFalse(input.CanAcceptConnection(function));
            Assert.IsFalse(input.CanAcceptConnection(result));
            Assert.IsFalse(input.CanAcceptConnection(null));
        }

        [TestMethod]
        public void Output()
        {
            var input = new Input(null);
            var property = new Property<int>("", "");
            var function = new TestFunction(() => { });
            var result = new Result("", "");
            var output = new OutputNeutral();

            Assert.IsTrue(output.CanAcceptConnection(function));
            Assert.IsFalse(output.CanAcceptConnection(input));
            Assert.IsFalse(output.CanAcceptConnection(property));
            Assert.IsFalse(output.CanAcceptConnection(result));
            Assert.IsFalse(output.CanAcceptConnection(output));
        }

        [TestMethod]
        public void Function()
        {
            var input = new Input(null);
            var property = new Property<int>("", "");
            var function = new TestFunction(() => { });
            var result = new Result("", "");
            var output = new OutputNeutral();

            Assert.IsTrue(function.CanAcceptConnection(input));
            Assert.IsFalse(function.CanAcceptConnection(property));
            Assert.IsFalse(function.CanAcceptConnection(function));
            Assert.IsFalse(function.CanAcceptConnection(result));
            Assert.IsFalse(function.CanAcceptConnection(output));
        }

        [TestMethod]
        public void Property()
        {
            var input = new Input(null);
            var property = new Property<int>("", "");
            var function = new TestFunction(() => { });
            var result = new Result("", "");
            var output = new OutputNeutral();

            Assert.IsTrue(property.CanAcceptConnection(result));
            Assert.IsFalse(property.CanAcceptConnection(input));
            Assert.IsFalse(property.CanAcceptConnection(property));
            Assert.IsFalse(property.CanAcceptConnection(function));
            Assert.IsFalse(property.CanAcceptConnection(output));
        }

        [TestMethod]
        public void Result()
        {
            var input = new Input(null);
            var property = new Property<int>("", "");
            var function = new TestFunction(() => { });
            var result = new Result("", "");
            var output = new OutputNeutral();

            Assert.IsTrue(result.CanAcceptConnection(function));
            Assert.IsFalse(result.CanAcceptConnection(input));
            Assert.IsFalse(result.CanAcceptConnection(property));
            Assert.IsFalse(result.CanAcceptConnection(result));
            Assert.IsFalse(result.CanAcceptConnection(output));
        }
    }
}
