using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core_Tests
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

            Assert.IsTrue(server.AddCommandListener(new UploadProgramServerCommand(server)));

            var expectedProgramDirectory = $"{StandardLocations.Instance.GetPathTo(Location.ApplicationDirectory)}{Path.DirectorySeparatorChar}Programs";
            var expectedSyncDirectory = $"{expectedProgramDirectory}{Path.DirectorySeparatorChar}Synced";

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));
            Assert.IsNull(client.LastMessageReceived);

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UPLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}This is the program name{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}This is the program contents"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsTrue(client.LastMessageReceived.IsSuccessMessage);

            client.LastMessageReceived = null;

            Assert.IsTrue(Directory.Exists(expectedProgramDirectory));
            Assert.IsTrue(Directory.Exists(expectedSyncDirectory));

            var files = Directory.GetFiles(expectedSyncDirectory);

            Assert.AreEqual(1, files.Length);
            Assert.AreEqual(Path.GetFileNameWithoutExtension(files[0]), "This is the program name");
            Assert.AreEqual($"This is the program contents", File.ReadAllText(files[0]));

            Assert.IsTrue(await client.Send($"some ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UPLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            await Task.Delay(TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(client.LastMessageReceived);
            Assert.IsFalse(client.LastMessageReceived.IsSuccessMessage);
            Assert.IsTrue(client.LastMessageReceived.IsFailureMessage);
            Assert.AreEqual($"The program received appeared to be invalid and can't be parsed.", client.LastMessageReceived.FailureMessage);

            Assert.IsTrue(await server.Stop());
        }
    }
}
