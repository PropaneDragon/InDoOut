using InDoOut_Executable_Core.Networking;
using InDoOut_Executable_Core.Programs;
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
            var server = new ProgramSyncServer(programHolder, 9001);
            var client = new ProgramSyncClient();

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            var programs = await client.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(0, programs.Count);

            var program = programHolder.NewProgram();
            program.SetName("First test program");

            Assert.AreEqual(1, programHolder.Programs.Count);

            programs = await client.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(1, programs.Count);
            Assert.AreEqual("First test program", programs[0]);

            program = programHolder.NewProgram();
            program.SetName("Another test program");

            Assert.AreEqual(2, programHolder.Programs.Count);

            programs = await client.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(2, programs.Count);
            Assert.AreEqual("First test program", programs[0]);
            Assert.AreEqual("Another test program", programs[1]);

            Assert.IsTrue(await server.Stop());
        }
    }
}
