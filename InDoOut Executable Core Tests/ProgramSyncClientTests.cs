using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core_Tests
{
    [TestClass]
    public class ProgramSyncClientTests
    {
        [TestMethod]
        public async Task RequestAvailablePrograms()
        {
            var server = new TestServer(9001);
            Assert.IsTrue(await server.Start());

            var client = new TestProgramSyncClient();
            Assert.IsTrue(await client.Connect(IPAddress.Loopback, 9001));

            Assert.IsNull(await client.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token));
            Assert.IsNotNull(server.LastMessageReceived);

            var splitMessage = server.LastMessageReceived.Split(TestProgramSyncClient.SplitIdentifier);

            Assert.AreEqual(2, splitMessage.Length);
            Assert.IsNotNull(Guid.Parse(splitMessage[0]));
            Assert.AreEqual("REQUEST_PROGRAMS\u0004\u0001\u0004", splitMessage[1]);

            var requestTask = client.RequestAvailablePrograms(new CancellationTokenSource(TimeSpan.FromMilliseconds(500)).Token);

            await Task.Delay(TimeSpan.FromMilliseconds(20));

            Assert.IsNotNull(server.LastMessageReceived);

            splitMessage = server.LastMessageReceived.Split(TestProgramSyncClient.SplitIdentifier);

            Assert.IsTrue(await server.SendMessageAll($"{splitMessage[0]}{TestProgramSyncClient.SplitIdentifier}REQUEST_PROGRAMS\u0004\u0001\u0004Program 1\nProgram 2\nAnother program\nLast program"));

            Assert.IsNotNull(requestTask.Result);
            Assert.AreEqual(4, requestTask.Result.Count);
            Assert.AreEqual("Program 1", requestTask.Result[0]);
            Assert.AreEqual("Program 2", requestTask.Result[1]);
            Assert.AreEqual("Another program", requestTask.Result[2]);
            Assert.AreEqual("Last program", requestTask.Result[3]);

            Assert.IsTrue(await server.Stop());
        }
    }
}
