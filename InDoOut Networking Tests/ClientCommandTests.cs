using InDoOut_Core.Entities.Functions;
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

            Assert.IsNull(await programRequestCommand.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));

            var lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.AreEqual("REQUEST_PROGRAMS", lastServerMessage.Name);

            var requestTask = programRequestCommand.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}Program 1{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}Program 2{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}Another program{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}Last program"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(4, requestTask.Result.Count);
            Assert.AreEqual("Program 1", requestTask.Result[0]);
            Assert.AreEqual("Program 2", requestTask.Result[1]);
            Assert.AreEqual("Another program", requestTask.Result[2]);
            Assert.AreEqual("Last program", requestTask.Result[3]);

            requestTask = programRequestCommand.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}Only one program"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(1, requestTask.Result.Count);
            Assert.AreEqual("Only one program", requestTask.Result[0]);

            requestTask = programRequestCommand.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(0, requestTask.Result.Count);

            requestTask = programRequestCommand.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}REQUEST_PROGRAMS{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}this is a program{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}this is another program"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(2, requestTask.Result.Count);
            Assert.AreEqual("this is a program", requestTask.Result[0]);
            Assert.AreEqual("this is another program", requestTask.Result[1]);

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

            var programUploadCommand = new UploadProgramClientCommand(client);
            var sendTask = programUploadCommand.SendProgram(program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            var lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));

            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}UPLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{NetworkCodes.COMMAND_SUCCESS_IDENTIFIER}{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}A message"));
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
            var downloadDataTask = programDownloadCommand.RequestDataForProgram("example-program", new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            var lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(program);
            Assert.IsNotNull(lastServerMessage);
            Assert.IsNotNull(Guid.Parse(lastServerMessage.Id));
            Assert.AreEqual("DOWNLOAD_PROGRAM", lastServerMessage.Name);
            Assert.AreEqual(1, lastServerMessage.Data.Length);
            Assert.AreEqual("example-program", lastServerMessage.Data[0]);

            var programData = File.ReadAllText("example-program.ido");

            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DOWNLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));
            Assert.IsNotNull(downloadDataTask.Result);
            Assert.AreEqual(downloadDataTask.Result, programData.Replace("\r", ""));


            var downloadProgramTask = programDownloadCommand.RequestProgram("example-program", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DOWNLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));
            Assert.IsFalse(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            downloadDataTask = programDownloadCommand.RequestDataForProgram("example-program", new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DOWNLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));
            Assert.IsNull(downloadDataTask.Result);


            downloadProgramTask = programDownloadCommand.RequestProgram("example-program", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DOWNLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}"));
            Assert.IsFalse(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            downloadDataTask = programDownloadCommand.RequestDataForProgram("example-program", new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DOWNLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}data{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}more data"));
            Assert.IsNull(downloadDataTask.Result);


            downloadProgramTask = programDownloadCommand.RequestProgram("example-program", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DOWNLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}data{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}more data"));
            Assert.IsFalse(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            downloadProgramTask = programDownloadCommand.RequestProgram("empty", program, new CancellationTokenSource(TimeSpan.FromMilliseconds(800)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(200));

            lastServerMessage = server.LastMessageReceived;
            server.LastMessageReceived = null;

            programData = File.ReadAllText("empty.ido");

            Assert.IsNotNull(lastServerMessage);
            Assert.IsTrue(await server.SendMessageAll($"{lastServerMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}DOWNLOAD_PROGRAM{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{programData}"));
            Assert.IsTrue(downloadProgramTask.Result);
            Assert.IsNotNull(program);


            Assert.IsTrue(await server.Stop());
        }
    }
}
