using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core.Programs;
using InDoOut_Function_Plugins.Loaders;
using InDoOut_Json_Storage;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class ClientServerEndToEndTests
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

        [TestMethod]
        public async Task UploadProgram()
        {
            var loadedPlugins = new LoadedPlugins();
            var programHolder = new ProgramHolder();
            var server = new Server(9001);
            var client = new Client();
            var uploadProgramClient = new UploadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var uploadProgramServer = new UploadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());

            Assert.IsTrue(server.AddCommandListener(uploadProgramServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            Assert.AreEqual(0, programHolder.Programs.Count);

            var result = await uploadProgramClient.SendProgram((IProgram)null, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsFalse(result);
            Assert.AreEqual(0, programHolder.Programs.Count);

            result = await uploadProgramClient.SendProgram((string)null, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsFalse(result);
            Assert.AreEqual(0, programHolder.Programs.Count);

            var program = new Program();

            program.Metadata["first metadata"] = "first";
            program.Metadata["second"] = "2";

            result = await uploadProgramClient.SendProgram(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsTrue(result);
            Assert.AreEqual(1, programHolder.Programs.Count);
            Assert.AreEqual(2, programHolder.Programs[0].Metadata.Count);
            Assert.AreEqual("first", programHolder.Programs[0].Metadata["first metadata"]);
            Assert.AreEqual("2", programHolder.Programs[0].Metadata["second"]);

            program = new Program();

            program.Metadata["second program"] = "yay";
            program.Metadata["another"] = "more metadata";

            result = await uploadProgramClient.SendProgram(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsTrue(result);
            Assert.AreEqual(2, programHolder.Programs.Count);
            Assert.AreEqual(2, programHolder.Programs[0].Metadata.Count);
            Assert.AreEqual("first", programHolder.Programs[0].Metadata["first metadata"]);
            Assert.AreEqual("2", programHolder.Programs[0].Metadata["second"]);
            Assert.AreEqual("yay", programHolder.Programs[1].Metadata["second program"]);
            Assert.AreEqual("more metadata", programHolder.Programs[1].Metadata["another"]);

            Assert.IsTrue(await server.Stop());
        }
    }
}
