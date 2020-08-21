using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Programs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core_Tests
{
    [TestClass]
    public class ProgramSyncServerTests
    {
        [TestMethod]
        public async Task SendAvailablePrograms()
        {
            var programHolder = new ProgramHolder();
            var server = new ProgramSyncServer(programHolder, 9001);
            var client = new TestClient();

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));
            Assert.IsNull(client.LastMessageReceived);

            Assert.IsTrue(await client.Send($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004", client.LastMessageReceived);

            client.LastMessageReceived = null;

            var program = programHolder.NewProgram();
            program.SetName("First test program");

            Assert.AreEqual(1, programHolder.Programs.Count);

            Assert.IsTrue(await client.Send($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004First test program", client.LastMessageReceived);

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004First test program", client.LastMessageReceived);

            program = programHolder.NewProgram();
            program.SetName("Second test program");

            client.LastMessageReceived = null;

            Assert.IsTrue(await client.Send($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004"));

            await Task.Delay(TimeSpan.FromMilliseconds(50));

            Assert.AreEqual($"some ID{ProgramSyncProgramCommandTracker.SPLIT_IDENTIFIER_STRING}REQUEST_PROGRAMS\u0004\u0001\u0004First test program\nSecond test program", client.LastMessageReceived);

            Assert.IsTrue(await server.Stop());
        }
    }
}
