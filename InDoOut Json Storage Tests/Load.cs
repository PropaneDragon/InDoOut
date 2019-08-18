using InDoOut_Core.Functions;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InDoOut_Json_Storage_Tests
{
    [TestClass]
    public class Load
    {
        [TestMethod]
        public void LoadJsonProgram()
        {
            var storer = new TestProgramJsonStorer("ExpectedJsonProgramFormat.json", new FunctionBuilder(), new LoadedPlugins());
            var jsonProgram = storer.LoadPublic();

            Assert.IsNotNull(jsonProgram);
            Assert.AreEqual(new Guid("12345678-1234-1234-1234-123456789abc"), jsonProgram.Id);
            Assert.AreEqual("second", jsonProgram.Metadata["first"]);
            Assert.AreEqual("fourth", jsonProgram.Metadata["third"]);
        }
    }
}
