using InDoOut_Application_Plugins.Self;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Core_Plugins.Finish;
using InDoOut_Core_Plugins.Maths;
using InDoOut_Core_Plugins.Start;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace InDoOut_Application_Plugins_Tests
{
    [TestClass]
    public class RunIDOProgramFunctionTests
    {
        [TestMethod]
        public void LoadAndReturnValue()
        {
            var values = new[] { "1", "2", "4" };
            var program = new Program(values);
            var start = new GenericStartFunction();
            var add1 = new AddFunction();
            var add2 = new AddFunction();
            var end = new GenericEndFunction();
            var pluginLoader = new FunctionPluginLoader();
            var loadedPlugins = new LoadedPlugins();
            var programName = "LoadProgramTest.ido";

            var programStream = new FileStream(programName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var storer = new ProgramJsonStorer(new FunctionBuilder(), loadedPlugins);

            var pluginContainer = pluginLoader.LoadPlugin("InDoOut Core Plugins.dll");

            Assert.IsTrue(pluginContainer.Initialise());
            Assert.IsTrue(pluginContainer.Valid);

            loadedPlugins.Plugins.Add(pluginContainer);

            Assert.IsTrue(program.AddFunction(start));
            Assert.IsTrue(program.AddFunction(add1));
            Assert.IsTrue(program.AddFunction(add2));
            Assert.IsTrue(program.AddFunction(end));

            Assert.IsTrue(start.GetOutputByName("Program started").Connect(add1.GetInputByName("Calculate")));
            Assert.IsTrue(add1.GetOutputByName("Calculated").Connect(add2.GetInputByName("Calculate")));
            Assert.IsTrue(add2.GetOutputByName("Calculated").Connect(end.GetInputByName("Return")));

            Assert.IsTrue(start.GetResultByName("Value 1").Connect(add1.GetPropertyByName("First number")));
            Assert.IsTrue(start.GetResultByName("Value 2").Connect(add1.GetPropertyByName("Second number")));
            Assert.IsTrue(add1.GetResultByName("Result value").Connect(add2.GetPropertyByName("First number")));
            Assert.IsTrue(start.GetResultByName("Value 3").Connect(add2.GetPropertyByName("Second number")));
            Assert.IsTrue(add2.GetResultByName("Result value").Connect(end.GetPropertyByName("Return value")));

            program.Trigger(null);
            program.WaitForCompletion();

            Assert.AreEqual("7", program.ReturnCode);

            Assert.AreEqual(0, storer.Save(program, programStream).Count);
            Assert.IsTrue(File.Exists(programName));

            programStream.Dispose();

            var runProgramFunction = new RunIDOProgramFunction() { PluginsToUse = loadedPlugins };
            runProgramFunction.GetPropertyByName("Path").RawValue = programName;
            runProgramFunction.GetPropertyByName("Value 1").RawValue = values[0];
            runProgramFunction.GetPropertyByName("Value 2").RawValue = values[1];
            runProgramFunction.GetPropertyByName("Value 3").RawValue = values[2];
            runProgramFunction.Trigger(null);
            runProgramFunction.WaitForCompletion();

            Assert.AreEqual("7", runProgramFunction.GetResultValue("Result"));

            File.Delete(programName);
        }
    }
}
