﻿using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Programs;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
            var server = new Server(9001);
            var client = new TestClient();

            Assert.IsTrue(server.AddCommandListener(new RequestProgramsServerCommand(server, programHolder)));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));
            Assert.IsNull(client.LastMessageReceived);

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("REQUEST_PROGRAMS", client.LastMessageReceived.Name);
            Assert.IsNull(client.LastMessageReceived.Data);

            client.LastMessageReceived = null;

            var program = programHolder.NewProgram();
            program.SetName("First test program");

            Assert.AreEqual(1, programHolder.Programs.Count);

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("REQUEST_PROGRAMS", client.LastMessageReceived.Name);
            Assert.AreEqual(1, client.LastMessageReceived.Data.Length);
            Assert.AreEqual("First test program", client.LastMessageReceived.Data[0]);

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("REQUEST_PROGRAMS", client.LastMessageReceived.Name);
            Assert.AreEqual(1, client.LastMessageReceived.Data.Length);
            Assert.AreEqual("First test program", client.LastMessageReceived.Data[0]);

            program = programHolder.NewProgram();
            program.SetName("Second test program");

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual("some ID", client.LastMessageReceived.Id);
            Assert.AreEqual("REQUEST_PROGRAMS", client.LastMessageReceived.Name);
            Assert.AreEqual(2, client.LastMessageReceived.Data.Length);
            Assert.AreEqual("First test program", client.LastMessageReceived.Data[0]);
            Assert.AreEqual("Second test program", client.LastMessageReceived.Data[1]);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task UploadProgram()
        {
            var server = new Server(9001);
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
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));
            Assert.IsNull(client.LastMessageReceived);
            Assert.AreEqual(0, programHolder.Programs.Count);


            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UPLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}This is the program contents"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual(0, programHolder.Programs.Count);

            client.LastMessageReceived = null;


            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UPLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.AreEqual(0, programHolder.Programs.Count);

            client.LastMessageReceived = null;

            var programData = File.ReadAllText("example-program.ido");

            Assert.IsNotNull(programData);
            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UPLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);

            client.LastMessageReceived = null;

            Assert.IsTrue(await server.Stop());
        }
    }
}
