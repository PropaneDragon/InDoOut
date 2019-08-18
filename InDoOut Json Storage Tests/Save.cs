using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core_Tests;
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
        public void Initialise()
        {
            _temporaryPath = Path.GetTempFileName();
        }

        [TestMethod]
        public void SaveFromJsonProgram()
        {
            File.WriteAllText(_temporaryPath, "");

            var jsonProgram = new JsonProgram()
            {
                Id = new Guid("12345678-1234-1234-1234-123456789abc"),
                Functions = new List<JsonFunction>()
                {
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Core_Tests.TestFunction, InDoOut Core Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-1111-1111-1111-123456789abc")
                    },
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Core_Tests.TestFunction, InDoOut Core Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-2222-2222-2222-123456789abc")
                    },
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Core_Tests.TestFunction, InDoOut Core Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-3333-3333-3333-123456789abc")
                    },
                    new JsonFunction()
                    {
                        FunctionClass = "InDoOut_Core_Tests.TestFunction, InDoOut Core Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
                        Id = new Guid("12345678-4444-4444-4444-123456789abc")
                    },
                },
                Metadata = new Dictionary<string, string>()
                {
                    { "first", "second" },
                    { "third", "fourth" }
                }
            };

            var storer = new TestProgramJsonStorer(_temporaryPath, new FunctionBuilder(), new LoadedPlugins());

            Assert.IsTrue(storer.SavePublic(jsonProgram));

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

            var firstFunction = new TestFunction() { Id = new Guid("12345678-1111-1111-1111-123456789abc") };
            var secondFunction = new TestFunction() { Id = new Guid("12345678-2222-2222-2222-123456789abc") };
            var thirdFunction = new TestFunction() { Id = new Guid("12345678-3333-3333-3333-123456789abc") };
            var fourthFunction = new TestFunction() { Id = new Guid("12345678-4444-4444-4444-123456789abc") };

            program.Metadata["first"] = "second";
            program.Metadata["third"] = "fourth";

            _ = program.AddFunction(firstFunction);
            _ = program.AddFunction(secondFunction);
            _ = program.AddFunction(thirdFunction);
            _ = program.AddFunction(fourthFunction);

            var storer = new TestProgramJsonStorer(_temporaryPath, new FunctionBuilder(), new LoadedPlugins());

            Assert.IsTrue(storer.Save(program));

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
