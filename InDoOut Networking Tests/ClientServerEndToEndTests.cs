using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Programs;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
using InDoOut_Networking.Server;
using InDoOut_Networking.Server.Commands;
using InDoOut_Networking.Shared;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
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
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new Client();
            var programRequestClient = new RequestProgramsClientCommand(client);
            var programRequestServer = new RequestProgramsServerCommand(server, programHolder);

            Assert.IsTrue(server.AddCommandListener(programRequestServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));

            var programs = await programRequestClient.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(0, programs.Count);

            var program = programHolder.NewProgram();
            program.SetName("First test program");
            var firstId = program.Id;

            Assert.AreEqual(1, programHolder.Programs.Count);

            programs = await programRequestClient.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(1, programs.Count);
            Assert.AreEqual(firstId, programs[0]);

            program = programHolder.NewProgram();
            program.SetName("Another test program");

            Assert.AreEqual(2, programHolder.Programs.Count);

            programs = await programRequestClient.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsNotNull(programs);
            Assert.AreEqual(2, programs.Count);
            Assert.AreEqual(firstId, programs[0]);
            Assert.AreEqual(program.Id, programs[1]);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task UploadProgram()
        {
            var loadedPlugins = new LoadedPlugins();
            var programHolder = new ProgramHolder();
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new Client();
            var uploadProgramClient = new UploadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var uploadProgramServer = new UploadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());

            Assert.IsTrue(server.AddCommandListener(uploadProgramServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));

            Assert.AreEqual(0, programHolder.Programs.Count);

            var result = await uploadProgramClient.SendProgramAsync((IProgram)null, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsFalse(result);
            Assert.AreEqual(0, programHolder.Programs.Count);

            result = await uploadProgramClient.SendProgramAsync((string)null, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            Assert.IsFalse(result);
            Assert.AreEqual(0, programHolder.Programs.Count);

            var program = new Program();

            program.SetName("First program");
            program.Metadata["first metadata"] = "first";
            program.Metadata["second"] = "2";

            result = await uploadProgramClient.SendProgramAsync(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

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

            result = await uploadProgramClient.SendProgramAsync(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

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
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new Client();
            var downloadProgramClient = new DownloadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var downloadProgramServer = new DownloadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());

            Assert.IsTrue(server.AddCommandListener(downloadProgramServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));

            Assert.AreEqual(0, programHolder.Programs.Count);

            var program = programHolder.NewProgram();
            program.SetName("A test program");
            program.Metadata["some metadata"] = "meta";
            program.Metadata["More metadata"] = "woop woop";

            Assert.AreEqual(1, programHolder.Programs.Count);

            var comparisonProgram = new Program();

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgramAsync(null, comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgramAsync("", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgramAsync("      ", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsFalse(await downloadProgramClient.RequestProgramAsync("Doesn't exist", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            Assert.AreEqual(0, comparisonProgram.Metadata.Count);
            Assert.AreEqual("Untitled", comparisonProgram.Name);

            Assert.IsTrue(await downloadProgramClient.RequestProgramAsync("A test program", comparisonProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

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
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new Client();
            var uploadProgramClient = new UploadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var uploadProgramServer = new UploadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());
            var downloadProgramClient = new DownloadProgramClientCommand(client, loadedPlugins, new FunctionBuilder());
            var downloadProgramServer = new DownloadProgramServerCommand(server, programHolder, loadedPlugins, new FunctionBuilder());

            Assert.IsTrue(server.AddCommandListener(uploadProgramServer));
            Assert.IsTrue(server.AddCommandListener(downloadProgramServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));

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

                Assert.IsTrue(await uploadProgramClient.SendProgramAsync(baseProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

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

                Assert.IsTrue(await downloadProgramClient.RequestProgramAsync(baseName, downloadedProgram, new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

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

        [TestMethod]
        public async Task GetProgramStatus()
        {
            var programHolder = new ProgramHolder();
            var port = PortFinder.Find();
            var server = new Server(port);
            var client = new Client();
            var programStatusClient = new GetProgramStatusClientCommand(client);
            var programStatusServer = new GetProgramStatusServerCommand(server, programHolder);

            Assert.IsTrue(server.AddCommandListener(programStatusServer));

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, port));

            Assert.AreEqual(0, programHolder.Programs.Count);

            var program1 = programHolder.NewProgram();
            var functionToRun1 = new InDoOut_Core_Tests.TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var functionToRun2 = new InDoOut_Core_Tests.TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));

            var program2 = programHolder.NewProgram();
            var functionToRun3 = new InDoOut_Core_Tests.TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));

            program1.SetName("Test program");
            program2.SetName("The other test program");

            Assert.IsTrue(program1.AddFunction(functionToRun1));
            Assert.IsTrue(program1.AddFunction(functionToRun2));

            Assert.IsTrue(program2.AddFunction(functionToRun3));

            var programStatus = await programStatusClient.GetProgramStatusAsync(program1.Id, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);

            Assert.IsNotNull(programStatus);
            Assert.AreEqual(program1.Id, programStatus.Id);
            Assert.AreEqual("Test program", programStatus.Name);
            Assert.AreEqual(2, programStatus.Functions.Count());
            Assert.IsFalse(programStatus.Running);

            functionToRun1.Trigger(null);
            Thread.Sleep(TimeSpan.FromMilliseconds(30));

            programStatus = await programStatusClient.GetProgramStatusAsync(program1.Id, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);

            Assert.IsNotNull(programStatus);
            Assert.AreEqual(program1.Id, programStatus.Id);
            Assert.AreEqual("Test program", programStatus.Name);
            Assert.AreEqual(2, programStatus.Functions.Count());
            Assert.AreEqual(functionToRun1.Id, programStatus.Functions[0].Id);
            Assert.AreEqual(State.Processing, programStatus.Functions[0].State);
            Assert.AreEqual(functionToRun1.LastTriggerTime, programStatus.Functions[0].LastTriggerTime);
            Assert.AreEqual(functionToRun1.LastCompletionTime, programStatus.Functions[0].LastCompletionTime);
            Assert.AreEqual(functionToRun2.Id, programStatus.Functions[1].Id);
            Assert.AreEqual(State.Waiting, programStatus.Functions[1].State);
            Assert.AreEqual(functionToRun2.LastTriggerTime, programStatus.Functions[1].LastTriggerTime);
            Assert.AreEqual(functionToRun2.LastCompletionTime, programStatus.Functions[1].LastCompletionTime);
            Assert.IsTrue(programStatus.Running);

            functionToRun2.Trigger(null);
            Thread.Sleep(TimeSpan.FromMilliseconds(30));

            programStatus = await programStatusClient.GetProgramStatusAsync(program1.Id, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);

            Assert.IsNotNull(programStatus);
            Assert.AreEqual(program1.Id, programStatus.Id);
            Assert.AreEqual("Test program", programStatus.Name);
            Assert.AreEqual(2, programStatus.Functions.Count());
            Assert.AreEqual(functionToRun1.Id, programStatus.Functions[0].Id);
            Assert.AreEqual(State.Processing, programStatus.Functions[0].State);
            Assert.AreEqual(functionToRun1.LastTriggerTime, programStatus.Functions[0].LastTriggerTime);
            Assert.AreEqual(functionToRun1.LastCompletionTime, programStatus.Functions[0].LastCompletionTime);
            Assert.AreEqual(functionToRun2.Id, programStatus.Functions[1].Id);
            Assert.AreEqual(State.Processing, programStatus.Functions[1].State);
            Assert.AreEqual(functionToRun2.LastTriggerTime, programStatus.Functions[1].LastTriggerTime);
            Assert.AreEqual(functionToRun2.LastCompletionTime, programStatus.Functions[1].LastCompletionTime);
            Assert.IsTrue(programStatus.Running);

            programStatus = await programStatusClient.GetProgramStatusAsync(program2.Id, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);

            Assert.IsNotNull(programStatus);
            Assert.AreEqual(program2.Id, programStatus.Id);
            Assert.AreEqual("The other test program", programStatus.Name);
            Assert.AreEqual(1, programStatus.Functions.Count());
            Assert.AreEqual(functionToRun3.Id, programStatus.Functions[0].Id);
            Assert.AreEqual(State.Waiting, programStatus.Functions[0].State);
            Assert.AreEqual(functionToRun3.LastTriggerTime, programStatus.Functions[0].LastTriggerTime);
            Assert.AreEqual(functionToRun3.LastCompletionTime, programStatus.Functions[0].LastCompletionTime);
            Assert.IsFalse(programStatus.Running);

            programStatus = await programStatusClient.GetProgramStatusAsync(Guid.NewGuid(), new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);

            Assert.IsNull(programStatus);

            programStatus = await programStatusClient.GetProgramStatusAsync(Guid.Empty, new CancellationTokenSource(TimeSpan.FromSeconds(2)).Token);

            Assert.IsNull(programStatus);

            Assert.IsTrue(await server.Stop());
        }
    }
}
