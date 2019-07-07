using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void CreateInputsOutputs()
        {
            var function = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(5)));

            Assert.AreEqual(0, function.Inputs.Count);
            Assert.AreEqual(0, function.Outputs.Count);

            var input = function.CreateInputPublic();
            var output = function.CreateOutputPublic();

            Assert.AreEqual(1, function.Inputs.Count);
            Assert.AreEqual(1, function.Outputs.Count);
            Assert.IsNotNull(input);
            Assert.IsNotNull(output);

            input = function.CreateInputPublic();
            output = function.CreateOutputPublic();

            Assert.AreEqual(1, function.Inputs.Count);
            Assert.AreEqual(1, function.Outputs.Count);
            Assert.IsNull(input);
            Assert.IsNull(output);

            input = function.CreateInputPublic("A");
            output = function.CreateOutputPublic("A");

            Assert.AreEqual(2, function.Inputs.Count);
            Assert.AreEqual(2, function.Outputs.Count);
            Assert.IsNotNull(input);
            Assert.IsNotNull(output);
            Assert.AreEqual("A", input.Name);
            Assert.AreEqual("A", output.Name);

            input = function.CreateInputPublic("B");
            output = function.CreateOutputPublic("C");

            Assert.AreEqual(3, function.Inputs.Count);
            Assert.AreEqual(3, function.Outputs.Count);
            Assert.IsNotNull(input);
            Assert.IsNotNull(output);
            Assert.AreEqual("B", input.Name);
            Assert.AreEqual("C", output.Name);
        }
    }
}
