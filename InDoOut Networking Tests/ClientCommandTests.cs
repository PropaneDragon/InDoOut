using InDoOut_Core.Entities.Programs;
using InDoOut_Core.Functions;
using InDoOut_Executable_Core.Networking;
using InDoOut_Json_Storage;
using InDoOut_Networking.Client;
using InDoOut_Networking.Client.Commands;
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
    public class ClientCommandTests
    {
        private static readonly string TemporarySaveLocation = $"{Path.DirectorySeparatorChar}Temp";

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(TemporarySaveLocation))
            {
                Directory.Delete(TemporarySaveLocation, true);
            }
        }

        [TestMethod]
        public async Task RequestAvailablePrograms()
        {
            var server = new TestServer(9001);
            Assert.IsTrue(await server.Start());

            var client = new Client();
            var programRequestCommand = new RequestProgramsClientCommand(client);

            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            Assert.IsNull(await programRequestCommand.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            var lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.AreEqual("RequestPrograms", lastServerMessage.Name);

            var requestTask = programRequestCommand.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{{11111111-2222-3333-4444-555555555555}}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{{22222222-2222-3333-4444-555555555555}}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{{55555555-2222-3333-6666-888888888888}}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}Last program"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(4, requestTask.Result.Count);
            Assert.AreEqual(Guid.Parse("{11111111-2222-3333-4444-555555555555}"), requestTask.Result[0]);
            Assert.AreEqual(Guid.Parse("{22222222-2222-3333-4444-555555555555}"), requestTask.Result[1]);
            Assert.AreEqual(Guid.Parse("{55555555-2222-3333-6666-888888888888}"), requestTask.Result[2]);
            Assert.AreEqual(Guid.Empty, requestTask.Result[3]);

            requestTask = programRequestCommand.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{{22222222-2222-3333-4444-555555555555}}"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(1, requestTask.Result.Count);
            Assert.AreEqual(Guid.Parse("{22222222-2222-3333-4444-555555555555}"), requestTask.Result[0]);

            requestTask = programRequestCommand.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(0, requestTask.Result.Count);

            requestTask = programRequestCommand.RequestAvailableProgramsAsync(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}RequestPrograms{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{{22222222-2222-3333-4444-555555555555}}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{{55555555-2222-3333-6666-888888888888}}"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(2, requestTask.Result.Count);
            Assert.AreEqual(Guid.Parse("{22222222-2222-3333-4444-555555555555}"), requestTask.Result[0]);
            Assert.AreEqual(Guid.Parse("{55555555-2222-3333-6666-888888888888}"), requestTask.Result[1]);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task UploadProgram()
        {
            var server = new TestServer(9001);
            Assert.IsTrue(await server.Start());

            var client = new Client();
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            var program = new Program();
            program.SetName("UploadProgramTest");

            Assert.IsTrue(program.AddFunction(new TestFunction()));
            Assert.IsTrue(program.AddFunction(new TestFunction()));
            Assert.IsTrue(program.AddFunction(new TestFunction()));

            Assert.AreEqual(3, program.Functions.Count);

            var programLocation = $"{TemporarySaveLocation}{Path.DirectorySeparatorChar}program.ido";

            _ = Directory.CreateDirectory(TemporarySaveLocation);

            var fileStream = new FileStream(programLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var storer = new ProgramJsonStorer(new FunctionBuilder(), new LoadedPlugins());

            Assert.AreEqual(0, storer.Save(program, fileStream).Count);

            fileStream.Dispose();

            var programUploadCommand = new UploadProgramClientCommand(client, new LoadedPlugins(), new FunctionBuilder());
            var sendTask = programUploadCommand.SendProgramAsync(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            var lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));

            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UploadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{NetworkCodes.COMMAND_SUCCESS_IDENTIFIER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}A message"));
            Assert.IsNotNull(sendTask.Result);
            Assert.IsTrue(sendTask.Result);

            var programContents = File.ReadAllText(programLocation);

            File.Delete(programLocation);

            Assert.IsNotNull(programContents);
            Assert.AreEqual(lastServerMessage.Data.Length, 1);
            Assert.AreEqual(programContents.Replace("\r\n", "\n"), lastServerMessage.Data[0]);

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task DownloadProgram()
        {
            var server = new TestServer(9001);
            Assert.IsTrue(await server.Start());

            var client = new Client();
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            var program = new Program();

            var programDownloadCommand = new DownloadProgramClientCommand(client, LoadedPlugins.Instance, new FunctionBuilder());
            var downloadDataTask = programDownloadCommand.RequestDataForProgramAsync("example-program", new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            var lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(program);
            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.AreEqual("DownloadProgram", lastServerMessage.Name);
            Assert.AreEqual(1, lastServerMessage.Data.Length);
            Assert.AreEqual("example-program", lastServerMessage.Data[0]);

            var programData = File.ReadAllText("example-program.ido");

            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));
            Assert.IsNotNull(downloadDataTask.Result);
            Assert.AreEqual(downloadDataTask.Result, programData.Replace("\r", ""));


            var downloadProgramTask = programDownloadCommand.RequestProgramAsync("example-program", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));
            Assert.IsFalse(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            downloadDataTask = programDownloadCommand.RequestDataForProgramAsync("example-program", new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));
            Assert.IsNull(downloadDataTask.Result);


            downloadProgramTask = programDownloadCommand.RequestProgramAsync("example-program", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));
            Assert.IsFalse(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            downloadDataTask = programDownloadCommand.RequestDataForProgramAsync("example-program", new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}data{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}more data"));
            Assert.IsNull(downloadDataTask.Result);


            downloadProgramTask = programDownloadCommand.RequestProgramAsync("example-program", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}data{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}more data"));
            Assert.IsFalse(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            downloadProgramTask = programDownloadCommand.RequestProgramAsync("empty", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            programData = File.ReadAllText("empty.ido");

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DownloadProgram{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));
            Assert.IsTrue(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            Assert.IsTrue(await server.Stop());
        }
    }
}
