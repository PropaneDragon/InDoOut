using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core_Tests
{
    [TestClass]
    public class ProgramSyncEndToEndTests
    {
        [TestMethod]
        public async Task AvailablePrograms()
        {
            var programHolder = new ProgramHolder();
            var server = new Server(9001);
            var client = new Client();
            var programRequestClient = new RequestProgramsClientCommand(client);
            var programRequestServer = new RequestProgramsServerCommand(server, programHolder);

            Assert.IsTrue(server.AddCommandListener(programRequestServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            var programs = await programRequestClient.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(0, programs.Count);

            var program = programHolder.NewProgram();
            program.SetName("First test program");

            Assert.AreEqual(1, programHolder.Programs.Count);

            programs = await programRequestClient.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(1, programs.Count);
            Assert.AreEqual("First test program", programs[0]);

            program = programHolder.NewProgram();
            program.SetName("Another test program");

            Assert.AreEqual(2, programHolder.Programs.Count);

            programs = await programRequestClient.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(2, programs.Count);
            Assert.AreEqual("First test program", programs[0]);
            Assert.AreEqual("Another test program", programs[1]);

            Assert.IsTrue(await server.Stop());
        }
    }
}
