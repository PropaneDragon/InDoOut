using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Desktop_API_Tests.External_Plugin_Testing;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace InDoOut_Json_Storage_Tests
{
    [TestClass]
    public class Save
    {
        private string _temporaryPath = null;

        [TestInitialize]
        public void Initialise() => _temporaryPath = Path.GetTempFileName();

        [TestMethod]
        public void SaveFromJsonProgram()
        {
            File.WriteAllText(_temporaryPath, "");

            var jsonProgram = new JsonProgram()
            {
                Name = "Test program",
                Id = new Guid("12345678-1234-1234-1234-123456789abc"),
                Functions = new List<JsonFunction>()
                {
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionA, InDoOut Desktop API Tests, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-1111-1111-1111-123456789abc")
                    },
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionB, InDoOut Desktop API Tests, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-2222-2222-2222-123456789abc")
                    },
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionB, InDoOut Desktop API Tests, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-3333-3333-3333-123456789abc")
                    },
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Desktop_API_Tests.External_Plugin_Testing.TestImportableFunctionA, InDoOut Desktop API Tests, Version=0.1.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-4444-4444-4444-123456789abc")
                    },
                },
                Connections = new List<JsonConnection>()
                {
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.InputOutput,
                        OutputName = "A Output 1",
                        InputName = "B Input 1",
                        StartFunctionId = new Guid("12345678-1111-1111-1111-123456789abc"),
                        EndFunctionId = new Guid("12345678-2222-2222-2222-123456789abc"),
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.InputOutput,
                        OutputName = "A Output 1",
                        InputName = "B Input 1",
                        StartFunctionId = new Guid("12345678-1111-1111-1111-123456789abc"),
                        EndFunctionId = new Guid("12345678-3333-3333-3333-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.PropertyResult,
                        OutputName = "A Result 1",
                        InputName = "A Property 2",
                        StartFunctionId = new Guid("12345678-1111-1111-1111-123456789abc"),
                        EndFunctionId = new Guid("12345678-4444-4444-4444-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.PropertyResult,
                        OutputName = "A Result 1",
                        InputName = "B Property 1",
                        StartFunctionId = new Guid("12345678-1111-1111-1111-123456789abc"),
                        EndFunctionId = new Guid("12345678-2222-2222-2222-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.PropertyResult,
                        OutputName = "A Result 2",
                        InputName = "B Property 1",
                        StartFunctionId = new Guid("12345678-1111-1111-1111-123456789abc"),
                        EndFunctionId = new Guid("12345678-3333-3333-3333-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.InputOutput,
                        OutputName = "B Output 1",
                        InputName = "B Input 1",
                        StartFunctionId = new Guid("12345678-2222-2222-2222-123456789abc"),
                        EndFunctionId = new Guid("12345678-3333-3333-3333-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.InputOutput,
                        OutputName = "B Output 2",
                        InputName = "B Input 1",
                        StartFunctionId = new Guid("12345678-2222-2222-2222-123456789abc"),
                        EndFunctionId = new Guid("12345678-2222-2222-2222-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.PropertyResult,
                        OutputName = "B Result 2",
                        InputName = "B Property 2",
                        StartFunctionId = new Guid("12345678-2222-2222-2222-123456789abc"),
                        EndFunctionId = new Guid("12345678-2222-2222-2222-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.InputOutput,
                        OutputName = "B Output 1",
                        InputName = "B Input 1",
                        StartFunctionId = new Guid("12345678-3333-3333-3333-123456789abc"),
                        EndFunctionId = new Guid("12345678-2222-2222-2222-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.InputOutput,
                        OutputName = "B Output 2",
                        InputName = "A Input 1",
                        StartFunctionId = new Guid("12345678-3333-3333-3333-123456789abc"),
                        EndFunctionId = new Guid("12345678-4444-4444-4444-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.PropertyResult,
                        OutputName = "B Result 1",
                        InputName = "A Property 2",
                        StartFunctionId = new Guid("12345678-3333-3333-3333-123456789abc"),
                        EndFunctionId = new Guid("12345678-1111-1111-1111-123456789abc")
                    },
                    new JsonConnection()
                    {
                        TypeOfConnection = JsonConnection.ConnectionType.PropertyResult,
                        OutputName = "B Result 2",
                        InputName = "A Property 2",
                        StartFunctionId = new Guid("12345678-3333-3333-3333-123456789abc"),
                        EndFunctionId = new Guid("12345678-1111-1111-1111-123456789abc")
                    },
                },
                PropertyValues = new List<JsonPropertyValue>()
                {
                    new JsonPropertyValue()
                    {
                        Value = "1",
                        Name = "A Property 1",
                        Function = new Guid("12345678-1111-1111-1111-123456789abc"),
                    },
                    new JsonPropertyValue()
                    {
                        Value = "First Property 2",
                        Name = "A Property 2",
                        Function = new Guid("12345678-1111-1111-1111-123456789abc"),
                    },
                    new JsonPropertyValue()
                    {
                        Value = "Second Property 1",
                        Name = "B Property 1",
                        Function = new Guid("12345678-2222-2222-2222-123456789abc"),
                    },
                    new JsonPropertyValue()
                    {
                        Value = "2",
                        Name = "B Property 2",
                        Function = new Guid("12345678-2222-2222-2222-123456789abc"),
                    },
                    new JsonPropertyValue()
                    {
                        Value = "Third Property 1",
                        Name = "B Property 1",
                        Function = new Guid("12345678-3333-3333-3333-123456789abc"),
                    },
                    new JsonPropertyValue()
                    {
                        Value = "3",
                        Name = "B Property 2",
                        Function = new Guid("12345678-3333-3333-3333-123456789abc"),
                    },
                    new JsonPropertyValue()
                    {
                        Value = "4",
                        Name = "A Property 1",
                        Function = new Guid("12345678-4444-4444-4444-123456789abc"),
                    },
                    new JsonPropertyValue()
                    {
                        Value = "Fourth Property 2",
                        Name = "A Property 2",
                        Function = new Guid("12345678-4444-4444-4444-123456789abc"),
                    },
                },
                Metadata = new Dictionary<string, string>()
                {
                    { "first", "second" },
                    { "third", "fourth" }
                }
            };

            var result = GenericJsonStorer.Save(jsonProgram, _temporaryPath);

            Assert.AreEqual(0, result.Count);

            var actualFileData = File.ReadAllText(_temporaryPath);
            var expectedFileData = File.ReadAllText("ExpectedJsonProgramFormat.json");

            Assert.AreEqual(expectedFileData, actualFileData);
        }

        [TestMethod]
        public void SaveFromRawProgram()
        {
            File.WriteAllText(_temporaryPath, "");

            var program = new Program()
            {
                Id = new Guid("12345678-1234-1234-1234-123456789abc")
            };

            var firstFunction = new TestImportableFunctionA() { Id = new Guid("12345678-1111-1111-1111-123456789abc") };
            var secondFunction = new TestImportableFunctionB() { Id = new Guid("12345678-2222-2222-2222-123456789abc") };
            var thirdFunction = new TestImportableFunctionB() { Id = new Guid("12345678-3333-3333-3333-123456789abc") };
            var fourthFunction = new TestImportableFunctionA() { Id = new Guid("12345678-4444-4444-4444-123456789abc") };

            Assert.IsTrue(firstFunction.AOutput1.Connect(secondFunction.BInput1));
            Assert.IsTrue(firstFunction.AOutput1.Connect(thirdFunction.BInput1));

            Assert.IsTrue(secondFunction.BOutput2.Connect(secondFunction.BInput1));
            Assert.IsTrue(secondFunction.BOutput1.Connect(thirdFunction.BInput1));

            Assert.IsTrue(thirdFunction.BOutput1.Connect(secondFunction.BInput1));
            Assert.IsTrue(thirdFunction.BOutput2.Connect(fourthFunction.AInput1));

            Assert.IsTrue(firstFunction.AResult1.Connect(fourthFunction.AProperty2));
            Assert.IsTrue(firstFunction.AResult1.Connect(secondFunction.BProperty1));
            Assert.IsTrue(firstFunction.AResult2.Connect(thirdFunction.BProperty1));

            Assert.IsTrue(secondFunction.BResult2.Connect(secondFunction.BProperty2));

            Assert.IsTrue(thirdFunction.BResult2.Connect(firstFunction.AProperty2));
            Assert.IsTrue(thirdFunction.BResult1.Connect(firstFunction.AProperty2));

            firstFunction.AProperty1.RawValue = "1";
            firstFunction.AProperty2.RawValue = "First Property 2";

            secondFunction.BProperty1.RawValue = "Second Property 1";
            secondFunction.BProperty2.RawValue = "2";

            thirdFunction.BProperty1.RawValue = "Third Property 1";
            thirdFunction.BProperty2.RawValue = "3";

            fourthFunction.AProperty1.RawValue = "4";
            fourthFunction.AProperty2.RawValue = "Fourth Property 2";

            program.SetName("Test program");

            program.Metadata["first"] = "second";
            program.Metadata["third"] = "fourth";

            _ = program.AddFunction(firstFunction);
            _ = program.AddFunction(secondFunction);
            _ = program.AddFunction(thirdFunction);
            _ = program.AddFunction(fourthFunction);

            var fileStream = new FileStream(_temporaryPath, FileMode.Truncate, FileAccess.Write);
            var jsonStorer = new ProgramJsonStorer(new FunctionBuilder(), new LoadedPlugins());
            var result = jsonStorer.Save(program, fileStream);

            fileStream.Dispose();

            Assert.AreEqual(0, result.Count);

            var actualFileData = File.ReadAllText(_temporaryPath);
            var expectedFileData = File.ReadAllText("ExpectedJsonProgramFormat.json");

            Assert.AreEqual(expectedFileData, actualFileData);
        }
        
        [TestCleanup]
        public void Cleanup()
        {
            if (!string.IsNullOrEmpty(_temporaryPath))
            {
                File.Delete(_temporaryPath);
            }
        }
    }
}
