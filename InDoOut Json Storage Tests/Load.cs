using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Desktop_API_Tests.External_Plugin_Testing;
using InDoOut_Json_Storage;
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

            Assert.AreEqual(12, jsonProgram.Connections.Count);

            var connectionIndex = 0;
            Assert.AreEqual(JsonConnection.ConnectionType.InputOutput, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("A Output 1", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.InputOutput, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("A Output 1", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.PropertyResult, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("A Result 1", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("A Property 2", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-4444-4444-4444-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.PropertyResult, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("A Result 1", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Property 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.PropertyResult, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("A Result 2", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Property 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.InputOutput, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("B Output 1", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.InputOutput, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("B Output 2", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.PropertyResult, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("B Result 2", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Property 2", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.InputOutput, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("B Output 1", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("B Input 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.InputOutput, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("B Output 2", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("A Input 1", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-4444-4444-4444-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.PropertyResult, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("B Result 1", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("A Property 2", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            ++connectionIndex;
            Assert.AreEqual(JsonConnection.ConnectionType.PropertyResult, jsonProgram.Connections[connectionIndex].TypeOfConnection);
            Assert.AreEqual("B Result 2", jsonProgram.Connections[connectionIndex].OutputName);
            Assert.AreEqual("A Property 2", jsonProgram.Connections[connectionIndex].InputName);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.Connections[connectionIndex].StartFunctionId);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.Connections[connectionIndex].EndFunctionId);

            Assert.AreEqual(8, jsonProgram.PropertyValues.Count);

            var propertyIndex = 0;
            Assert.AreEqual("1", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("A Property 1", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

            ++propertyIndex;
            Assert.AreEqual("First Property 2", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("A Property 2", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-1111-1111-1111-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

            ++propertyIndex;
            Assert.AreEqual("Second Property 1", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("B Property 1", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

            ++propertyIndex;
            Assert.AreEqual("2", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("B Property 2", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-2222-2222-2222-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

            ++propertyIndex;
            Assert.AreEqual("Third Property 1", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("B Property 1", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

            ++propertyIndex;
            Assert.AreEqual("3", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("B Property 2", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-3333-3333-3333-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

            ++propertyIndex;
            Assert.AreEqual("4", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("A Property 1", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-4444-4444-4444-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

            ++propertyIndex;
            Assert.AreEqual("Fourth Property 2", jsonProgram.PropertyValues[propertyIndex].Value);
            Assert.AreEqual("A Property 2", jsonProgram.PropertyValues[propertyIndex].Name);
            Assert.AreEqual(new Guid("12345678-4444-4444-4444-123456789abc"), jsonProgram.PropertyValues[propertyIndex].Function);

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
            Assert.AreEqual(0, storer.Load(program).Count);
            Assert.AreEqual(new Guid("12345678-1234-1234-1234-123456789abc"), program.Id);

            Assert.AreEqual("second", program.Metadata["first"]);
            Assert.AreEqual("fourth", program.Metadata["third"]);

            Assert.AreEqual(4, program.Functions.Count);

            var function1 = program.Functions[0] as TestImportableFunctionA;
            var function2 = program.Functions[1] as TestImportableFunctionB;
            var function3 = program.Functions[2] as TestImportableFunctionB;
            var function4 = program.Functions[3] as TestImportableFunctionA;

            Assert.IsTrue(function1 is TestImportableFunctionA);
            Assert.IsTrue(function2 is TestImportableFunctionB);
            Assert.IsTrue(function3 is TestImportableFunctionB);
            Assert.IsTrue(function4 is TestImportableFunctionA);

            Assert.AreEqual(2, function1.AOutput1.Connections.Count);
            Assert.AreEqual(function2.BInput1, function1.AOutput1.Connections[0]);
            Assert.AreEqual(function3.BInput1, function1.AOutput1.Connections[1]);
            Assert.AreEqual(function2, function1.AOutput1.Connections[0].Parent);
            Assert.AreEqual(function3, function1.AOutput1.Connections[1].Parent);

            Assert.AreEqual(2, function1.AResult1.Connections.Count);
            Assert.AreEqual(function4.AProperty2, function1.AResult1.Connections[0]);
            Assert.AreEqual(function2.BProperty1, function1.AResult1.Connections[1]);

            Assert.AreEqual(1, function1.AResult2.Connections.Count);
            Assert.AreEqual(function3.BProperty1, function1.AResult2.Connections[0]);

            Assert.AreEqual(1, function2.BOutput1.Connections.Count);
            Assert.AreEqual(1, function2.BOutput2.Connections.Count);
            Assert.AreEqual(function3.BInput1, function2.BOutput1.Connections[0]);
            Assert.AreEqual(function2.BInput1, function2.BOutput2.Connections[0]);
            Assert.AreEqual(function3, function2.BOutput1.Connections[0].Parent);
            Assert.AreEqual(function2, function2.BOutput2.Connections[0].Parent);

            Assert.AreEqual(1, function2.BResult2.Connections.Count);
            Assert.AreEqual(function2.BProperty2, function2.BResult2.Connections[0]);

            Assert.AreEqual(1, function3.BOutput1.Connections.Count);
            Assert.AreEqual(1, function3.BOutput2.Connections.Count);
            Assert.AreEqual(function2.BInput1, function3.BOutput1.Connections[0]);
            Assert.AreEqual(function4.AInput1, function3.BOutput2.Connections[0]);
            Assert.AreEqual(function2, function3.BOutput1.Connections[0].Parent);
            Assert.AreEqual(function4, function3.BOutput2.Connections[0].Parent);

            Assert.AreEqual(1, function3.BResult2.Connections.Count);
            Assert.AreEqual(function1.AProperty2, function3.BResult2.Connections[0]);

            Assert.AreEqual(1, function3.BResult1.Connections.Count);
            Assert.AreEqual(function1.AProperty2, function3.BResult1.Connections[0]);

            Assert.AreEqual("1", function1.AProperty1.RawValue);
            Assert.AreEqual("First Property 2", function1.AProperty2.RawValue);

            Assert.AreEqual("Second Property 1", function2.BProperty1.RawValue);
            Assert.AreEqual("2", function2.BProperty2.RawValue);

            Assert.AreEqual("Third Property 1", function3.BProperty1.RawValue);
            Assert.AreEqual("3", function3.BProperty2.RawValue);

            Assert.AreEqual("4", function4.AProperty1.RawValue);
            Assert.AreEqual("Fourth Property 2", function4.AProperty2.RawValue);
        }
    }
}
