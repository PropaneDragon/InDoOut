using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Location;
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

            Assert.AreEqual(4, jsonProgram.Functions.Count);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Functions[0].Id);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Functions[1].Id);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Functions[2].Id);
            Assert.AreEqual(new Guid("12345678-4444-4444-4444-123456789abc"), jsonProgram.Functions[3].Id);
            Assert.AreEqual("InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionA, InDoOut Desktop API Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", jsonProgram.Functions[0].FunctionClass);
            Assert.AreEqual("InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionB, InDoOut Desktop API Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", jsonProgram.Functions[1].FunctionClass);
            Assert.AreEqual("InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionB, InDoOut Desktop API Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", jsonProgram.Functions[2].FunctionClass);
            Assert.AreEqual("InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionA, InDoOut Desktop API Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", jsonProgram.Functions[3].FunctionClass);

            Assert.AreEqual(6, jsonProgram.Connections.Count);

            Assert.AreEqual("A Output 1", jsonProgram.Connections[0].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[0].InputName);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[0].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[0].EndFunctionId);

            Assert.AreEqual("A Output 1", jsonProgram.Connections[1].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[1].InputName);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[1].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[1].EndFunctionId);

            Assert.AreEqual("B Output 1", jsonProgram.Connections[2].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[2].InputName);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[2].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[2].EndFunctionId);

            Assert.AreEqual("B Output 2", jsonProgram.Connections[3].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[3].InputName);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[3].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[3].EndFunctionId);

            Assert.AreEqual("B Output 1", jsonProgram.Connections[4].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[4].InputName);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[4].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[4].EndFunctionId);

            Assert.AreEqual("B Output 2", jsonProgram.Connections[5].OutputName);
            Assert.AreEqual("A Input 1", jsonProgram.Connections[5].InputName);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[5].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-4444-4444-4444-123456789abc"), jsonProgram.Connections[5].EndFunctionId);

            Assert.AreEqual("second", jsonProgram.Metadata["first"]);
            Assert.AreEqual("fourth", jsonProgram.Metadata["third"]);
        }

        [TestMethod]
        public void LoadProgram()
        {
            var pluginLoader = new PluginLoader();
            var loadedPlugin = pluginLoader.LoadPlugin("InDoOut Desktop API Tests.dll");

            Assert.IsTrue(loadedPlugin.Initialise());

            var loadedPlugins = new LoadedPlugins();
            loadedPlugins.Plugins.Add(loadedPlugin);

            var storer = new TestProgramJsonStorer("ExpectedJsonProgramFormat.json", new FunctionBuilder(), loadedPlugins);
            var program = new Program();

            Assert.AreNotEqual(new Guid("12345678-1234-1234-1234-123456789abc"), program.Id);
            Assert.IsTrue(storer.Load(program));
            Assert.AreEqual(new Guid("12345678-1234-1234-1234-123456789abc"), program.Id);

        }
    }
}
