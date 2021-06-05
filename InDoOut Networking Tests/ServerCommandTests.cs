using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Programs;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using InDoOut_Networking.Shared;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class ServerCommandTests
    {
        [TestCleanup]
        public void Cleanup()
        {
            var expectedProgramDirectory = $"{StandardLocations.Instance.GetPathTo(Location.ApplicationDirectory)}{Path.DirectorySeparatorChar}Programs";

            if (Directory.Exists(expectedProgramDirectory))
            {
                Directory.Delete(expectedProgramDirectory, true);
            }
        }

        [TestMethod]
        public async Task SendAvailablePrograms()
        {
            var programHolder = new ProgramHolder();
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new TestClient();

            Assert.IsTrue(server.AddCommandListener(new RequestProgramsServerCommand(server, programHolder)));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));
            Assert.IsNull(client.LastMessageReceived);

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("RequestPrograms", client.LastMessageReceived.Name);
            Assert.IsNull(client.LastMessageReceived.Data);

            client.LastMessageReceived = null;

            var program = programHolder.NewProgram();
            program.SetName("First test program");
            var firstId = program.Id.ToString();

            Assert.AreEqual(1, programHolder.Programs.Count);

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("RequestPrograms", client.LastMessageReceived.Name);
            Assert.AreEqual(1, client.LastMessageReceived.Data.Length);
            Assert.AreEqual(firstId, client.LastMessageReceived.Data[0]);

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("RequestPrograms", client.LastMessageReceived.Name);
            Assert.AreEqual(1, client.LastMessageReceived.Data.Length);
            Assert.AreEqual(firstId, client.LastMessageReceived.Data[0]);

            program = programHolder.NewProgram();
            program.SetName("Second test program");

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("RequestPrograms", client.LastMessageReceived.Name);
            Assert.AreEqual(2, client.LastMessageReceived.Data.Length);
            Assert.AreEqual(firstId, client.LastMessageReceived.Data[0]);
            Assert.AreEqual(program.Id.ToString(), client.LastMessageReceived.Data[1]);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task UploadProgram()
        {
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new TestClient();
            var programHolder = new ProgramHolder();

            Assert.IsTrue(StandardLocations.Instance.SetPathTo(Location.PluginsDirectory, StandardLocations.Instance.GetPathTo(Location.ApplicationDirectory)));

            var pluginLoader = new PluginDirectoryLoader(new FunctionPluginLoader(), StandardLocations.Instance);
            var loadedPlugins = new LoadedPlugins
            {
                Plugins = await pluginLoader.LoadPlugins()
            };

            Assert.IsTrue(server.AddCommandListener(new UploadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder())));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));
            Assert.IsNull(client.LastMessageReceived);
            Assert.AreEqual(0, programHolder.Programs.Count);


            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UploadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}This is the program contents"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual("The program couldn't be loaded onto the server with the following failures:\n\n- Critical: The program contents could not be loaded.", client.LastMessageReceived.FailureMessage);
            Assert.AreEqual(0, programHolder.Programs.Count);

            client.LastMessageReceived = null;


            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UploadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual("The program received appeared to be invalid and can't be parsed.", client.LastMessageReceived.FailureMessage);
            Assert.AreEqual(0, programHolder.Programs.Count);

            client.LastMessageReceived = null;


            var programData = File.ReadAllText("example-program.ido");

            Assert.IsNotNull(programData);
            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UploadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual(35214, client.LastMessageReceived.FailureMessage.Length);
            Assert.AreEqual(0, programHolder.Programs.Count);

            client.LastMessageReceived = null;


            programData = File.ReadAllText("empty.ido");

            Assert.IsNotNull(programData);
            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UploadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsFalse(client.LastMessageReceived.IsFailureMessage);
            Assert.IsTrue(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual(1, programHolder.Programs.Count);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task DownloadProgram()
        {
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new TestClient();
            var programHolder = new ProgramHolder();

            Assert.IsTrue(server.AddCommandListener(new DownloadProgramServerCommand(server, programHolder, new LoadedPlugins(), new FunctionBuilder())));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));
            Assert.IsNull(client.LastMessageReceived);
            Assert.AreEqual(0, programHolder.Programs.Count);

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}A non existant program name"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.AreEqual("The program with the name \"A non existant program name\" doesn't exist on the server.", client.LastMessageReceived.FailureMessage);

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER} "));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.AreEqual("The program name requested was empty.", client.LastMessageReceived.FailureMessage);

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.AreEqual("The request appears to be invalid and can't be accepted by the server.", client.LastMessageReceived.FailureMessage);

            client.LastMessageReceived = null;

            var program2 = programHolder.NewProgram();
            program2.SetName("A program name 2 functions");

            var functionWithMetadata = new TestNetworkingFunction();
            functionWithMetadata.Metadata["this is meta"] = "Yes";

            Assert.IsTrue(program2.AddFunction(functionWithMetadata));
            Assert.IsTrue(program2.AddFunction(new TestNetworkingFunction()));

            var program1 = programHolder.NewProgram();
            program1.SetName("A program name 1 function");

            Assert.IsTrue(program1.AddFunction(new TestNetworkingFunction()));

            var program0 = programHolder.NewProgram();
            program0.SetName("0 functions");

            Assert.AreEqual(3, programHolder.Programs.Count);

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}A program"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.AreEqual("The program with the name \"A program\" doesn't exist on the server.", client.LastMessageReceived.FailureMessage);

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}A program name 2 functions"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsFalse(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual(1, client.LastMessageReceived.Data.Length);

            var parsed = JObject.Parse(client.LastMessageReceived.Data[0]);

            Assert.AreEqual(2, parsed["functions"].Children().Count());
            Assert.AreEqual("Yes", parsed["functions"].Children().First()["metadata"]["this is meta"].Value<string>());

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}A program name 1 function"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsFalse(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual(1, client.LastMessageReceived.Data.Length);

            parsed = JObject.Parse(client.LastMessageReceived.Data[0]);

            Assert.AreEqual(1, parsed["functions"].Children().Count());

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}0 functions"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsFalse(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual(1, client.LastMessageReceived.Data.Length);

            parsed = JObject.Parse(client.LastMessageReceived.Data[0]);

            Assert.AreEqual(0, parsed["functions"].Children().Count());

            client.LastMessageReceived = null;

            Assert.IsTrue(await server.Stop());
        }
    }
}
