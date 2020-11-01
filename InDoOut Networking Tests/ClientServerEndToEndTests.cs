using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

            program.SetName("First program");
            program.Metadata["first metadata"] = "first";
            program.Metadata["second"] = "2";

            result = await uploadProgramClient.SendProgram(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsTrue(result);
            Assert.AreEqual(1, programHolder.Programs.Count);
            Assert.AreEqual("First program", programHolder.Programs[0].Name);
            Assert.AreEqual(2, programHolder.Programs[0].Metadata.Count);
            Assert.AreEqual("first", programHolder.Programs[0].Metadata["first metadata"]);
            Assert.AreEqual("2", programHolder.Programs[0].Metadata["second"]);

            program = new Program();

            program.SetName("The other program");
            program.Metadata["second program"] = "yay";
            program.Metadata["another"] = "more metadata";

            result = await uploadProgramClient.SendProgram(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsTrue(result);
            Assert.AreEqual(2, programHolder.Programs.Count);
            Assert.AreEqual("First program", programHolder.Programs[0].Name);
            Assert.AreEqual(2, programHolder.Programs[0].Metadata.Count);
            Assert.AreEqual("first", programHolder.Programs[0].Metadata["first metadata"]);
            Assert.AreEqual("2", programHolder.Programs[0].Metadata["second"]);
            Assert.AreEqual("The other program", programHolder.Programs[1].Name);
            Assert.AreEqual(2, programHolder.Programs[1].Metadata.Count);
            Assert.AreEqual("yay", programHolder.Programs[1].Metadata["second program"]);
            Assert.AreEqual("more metadata", programHolder.Programs[1].Metadata["another"]);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task DownloadProgram()
        {
            var loadedPlugins = new LoadedPlugins();
            var programHolder = new ProgramHolder();
            var server = new Server(9001);
            var client = new Client();
            var downloadProgramClient = new DownloadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var downloadProgramServer = new DownloadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());

            Assert.IsTrue(server.AddCommandListener(downloadProgramServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            Assert.AreEqual(0, programHolder.Programs.Count);

            var program = programHolder.NewProgram();
            program.SetName("A test program");
            program.Metadata["some metadata"] = "meta";
            program.Metadata["More metadata"] = "woop woop";

            Assert.AreEqual(1, programHolder.Programs.Count);

            var comparisonProgram = new Program();

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgram(null, comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgram("", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgram("      ", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgram("Doesn't exist", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsTrue(await downloadProgramClient.RequestProgram("A test program", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(2, comparisonProgram.Metadata.Count);
            Assert.AreEqual("A test program", comparisonProgram.Name);
            Assert.AreEqual("meta", comparisonProgram.Metadata["some metadata"]);
            Assert.AreEqual("woop woop", comparisonProgram.Metadata["More metadata"]);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task UploadDownloadProgram()
        {
            var loadedPlugins = new LoadedPlugins();
            var programHolder = new ProgramHolder();
            var server = new Server(9001);
            var client = new Client();
            var uploadProgramClient = new UploadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var uploadProgramServer = new UploadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());
            var downloadProgramClient = new DownloadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var downloadProgramServer = new DownloadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());

            Assert.IsTrue(server.AddCommandListener(uploadProgramServer));
            Assert.IsTrue(server.AddCommandListener(downloadProgramServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            Assert.AreEqual(0, programHolder.Programs.Count);

            for (var count = 0; count < 10; ++count)
            {
                var baseProgram = new Program();
                var baseName = Guid.NewGuid().ToString();
                var baseMetadataCount = new Random().Next(1, 10);

                baseProgram.SetName(baseName);

                for (var metadataCount = 0; metadataCount < baseMetadataCount; ++metadataCount)
                {
                    baseProgram.Metadata[Guid.NewGuid().ToString()] = Guid.NewGuid().ToString();
                }

                Assert.IsTrue(await uploadProgramClient.SendProgram(baseProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

                Assert.AreEqual(count + 1, programHolder.Programs.Count);

                var uploadedProgram = programHolder.Programs[count];

                Assert.AreEqual(baseProgram.Name, uploadedProgram.Name);
                Assert.AreEqual(baseProgram.Id, uploadedProgram.Id);
                Assert.AreEqual(baseProgram.Metadata.Count, uploadedProgram.Metadata.Count);

                foreach (var metadata in baseProgram.Metadata)
                {
                    Assert.IsTrue(uploadedProgram.Metadata.ContainsKey(metadata.Key));
                    Assert.AreEqual(metadata.Value, uploadedProgram.Metadata[metadata.Key]);
                }

                var downloadedProgram = new Program();

                Assert.IsTrue(await downloadProgramClient.RequestProgram(baseName, downloadedProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

                Assert.AreEqual(baseProgram.Name, downloadedProgram.Name);
                Assert.AreEqual(baseProgram.Id, downloadedProgram.Id);
                Assert.AreEqual(baseProgram.Metadata.Count, downloadedProgram.Metadata.Count);

                foreach (var metadata in downloadedProgram.Metadata)
                {
                    Assert.IsTrue(downloadedProgram.Metadata.ContainsKey(metadata.Key));
                    Assert.AreEqual(metadata.Value, downloadedProgram.Metadata[metadata.Key]);
                }
            }

            Assert.IsTrue(await server.Stop());
        }
    }
}
