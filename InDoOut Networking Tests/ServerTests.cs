using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class ServerTests
    {
        [TestMethod]
        public async Task ServerStartStop()
        {
            var server = new TestServer(9002);

            Assert.IsNull(server.LastClientConnected);
            Assert.IsNull(server.LastClientDisconnected);
            Assert.IsNull(server.LastClientReceived);
            Assert.IsNull(server.LastRawMessageReceived);

            Assert.IsTrue(server.CanAcceptClients);
            Assert.IsFalse(server.Started);

            Assert.AreEqual(IPAddress.Any, server.IPAddress);
            Assert.AreEqual(9002, server.Port);
            Assert.AreEqual(0, server.Clients.Count);

            Assert.IsTrue(await server.Start());

            Assert.IsNull(server.LastClientConnected);
            Assert.IsNull(server.LastClientDisconnected);
            Assert.IsNull(server.LastClientReceived);
            Assert.IsNull(server.LastRawMessageReceived);

            Assert.IsTrue(server.CanAcceptClients);
            Assert.IsTrue(server.Started);

            Assert.AreEqual(IPAddress.Any, server.IPAddress);
            Assert.AreEqual(9002, server.Port);
            Assert.AreEqual(0, server.Clients.Count);

            Assert.IsTrue(await server.Stop());

            Assert.IsNull(server.LastClientConnected);
            Assert.IsNull(server.LastClientDisconnected);
            Assert.IsNull(server.LastClientReceived);
            Assert.IsNull(server.LastRawMessageReceived);

            Assert.IsTrue(server.CanAcceptClients);
            Assert.IsFalse(server.Started);

            Assert.AreEqual(IPAddress.Any, server.IPAddress);
            Assert.AreEqual(9002, server.Port);
            Assert.AreEqual(0, server.Clients.Count);

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await server.Start());

            Assert.IsNull(server.LastClientConnected);
            Assert.IsNull(server.LastClientDisconnected);
            Assert.IsNull(server.LastClientReceived);
            Assert.IsNull(server.LastRawMessageReceived);

            Assert.IsTrue(server.CanAcceptClients);
            Assert.IsTrue(server.Started);

            Assert.AreEqual(IPAddress.Any, server.IPAddress);
            Assert.AreEqual(9002, server.Port);
            Assert.AreEqual(0, server.Clients.Count);

            Assert.IsTrue(await server.Stop());

            Assert.IsNull(server.LastClientConnected);
            Assert.IsNull(server.LastClientDisconnected);
            Assert.IsNull(server.LastClientReceived);
            Assert.IsNull(server.LastRawMessageReceived);

            Assert.IsTrue(server.CanAcceptClients);
            Assert.IsFalse(server.Started);

            Assert.AreEqual(IPAddress.Any, server.IPAddress);
            Assert.AreEqual(9002, server.Port);
            Assert.AreEqual(0, server.Clients.Count);
        }

        [TestMethod]
        public async Task ClientConnectDisconnect()
        {
            var server = new TestServer(9003) { ClientPollInterval = TimeSpan.FromMilliseconds(10) };
            var clientA = new TestClient();
            var clientB = new TestClient();

            Assert.IsTrue(await server.Start());

            Assert.IsNull(server.LastClientConnected);
            Assert.IsNull(server.LastClientDisconnected);
            Assert.IsNull(server.LastClientReceived);
            Assert.IsNull(server.LastRawMessageReceived);

            Assert.IsTrue(server.CanAcceptClients);
            Assert.IsTrue(server.Started);

            Assert.AreEqual(0, server.Clients.Count);

            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsFalse(clientA.Connected);
            Assert.IsFalse(clientB.Connected);

            Assert.IsFalse(await clientA.Connect(IPAddress.Loopback, -1));
            Assert.IsFalse(await clientA.Connect(IPAddress.Loopback, -5));
            Assert.IsFalse(await clientA.Connect(IPAddress.Loopback, 0));
            Assert.IsFalse(await clientA.Connect(IPAddress.Loopback, 5615));
            Assert.IsFalse(await clientA.Connect(IPAddress.Loopback, 80));
            Assert.IsFalse(await clientA.Connect(IPAddress.Loopback, 9004));
            Assert.IsTrue(await clientA.Connect(IPAddress.Loopback, 9003));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(1, server.Clients.Count);
            Assert.IsNotNull(server.LastClientConnected);
            Assert.IsNull(server.LastClientDisconnected);

            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(clientA.Connected);
            Assert.IsFalse(clientB.Connected);

            Assert.IsTrue(await clientA.Disconnect());

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.IsFalse(clientA.Connected);
            Assert.IsFalse(clientB.Connected);

            Assert.AreEqual(0, server.Clients.Count);
            Assert.IsNotNull(server.LastClientConnected);
            Assert.IsNotNull(server.LastClientDisconnected);

            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await clientA.Connect(IPAddress.Loopback, 9003));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(1, server.Clients.Count);

            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(clientA.Connected);
            Assert.IsFalse(clientB.Connected);

            Assert.IsTrue(await clientB.Connect(IPAddress.Loopback, 9003));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(2, server.Clients.Count);

            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(clientA.Connected);
            Assert.IsTrue(clientB.Connected);

            Assert.IsTrue(await clientA.Disconnect());

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(1, server.Clients.Count);

            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsFalse(clientA.Connected);
            Assert.IsTrue(clientB.Connected);

            Assert.IsTrue(await server.Stop());

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(0, server.Clients.Count);
        }

        [TestMethod]
        public async Task ClientSendReceive()
        {
            var server = new TestServer(9004) { ClientPollInterval = TimeSpan.FromMilliseconds(10) };
            var clientA = new TestClient();
            var clientB = new TestClient();

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await server.SendMessageAll("This is a test and shouldn't be received by anyone!"));
            Assert.IsFalse(await clientA.Send("This is a test and shouldn't be received by anyone!"));
            Assert.IsFalse(await clientB.Send("This is a test and shouldn't be received by anyone!"));

            Assert.IsTrue(await clientA.Connect(IPAddress.Loopback, 9004));
            Assert.IsTrue(await clientB.Connect(IPAddress.Loopback, 9004));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual(2, server.Clients.Count);

            Assert.IsTrue(clientA.Connected);
            Assert.IsTrue(clientB.Connected);

            Assert.IsNull(server.LastRawMessageReceived);
            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await clientA.Send("This is client A!"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client A!", server.LastRawMessageReceived);
            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await clientB.Send("This is client B!"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client B!", server.LastRawMessageReceived);
            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await clientA.Send("This is client A again!"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client A again!", server.LastRawMessageReceived);
            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await clientA.Send("This is client A again again!"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client A again again!", server.LastRawMessageReceived);
            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await clientB.Send("This is client B again!"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client B again!", server.LastRawMessageReceived);
            Assert.IsNull(clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await server.SendMessage(server.Clients.ElementAt(0), "Hello client A"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client B again!", server.LastRawMessageReceived);
            Assert.AreEqual("Hello client A", clientA.LastRawMessageReceived);
            Assert.IsNull(clientB.LastRawMessageReceived);

            Assert.IsTrue(await server.SendMessage(server.Clients.ElementAt(1), "Hello client B"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client B again!", server.LastRawMessageReceived);
            Assert.AreEqual("Hello client A", clientA.LastRawMessageReceived);
            Assert.AreEqual("Hello client B", clientB.LastRawMessageReceived);

            Assert.IsTrue(await server.SendMessageAll("Hello all clients!"));

            await Task.Delay(TimeSpan.FromMilliseconds(100));

            Assert.AreEqual("This is client B again!", server.LastRawMessageReceived);
            Assert.AreEqual("Hello all clients!", clientA.LastRawMessageReceived);
            Assert.AreEqual("Hello all clients!", clientB.LastRawMessageReceived);

            Assert.IsFalse(await server.SendMessageAll(null));
            Assert.IsFalse(await server.SendMessageAll(""));
            Assert.IsFalse(await server.SendMessage(null, "Hello null client"));

            Assert.IsTrue(await server.Stop());
        }

        [TestMethod]
        public async Task SendReceiveUTF8()
        {
            var server = new TestServer(9005);
            var clientA = new TestClient();

            Assert.IsTrue(await server.Start());
            Assert.IsTrue(await clientA.Connect(IPAddress.Loopback, 9005));

            var testTextFiles = Directory.GetFiles(".", "*.txt");
            Assert.AreEqual(5, testTextFiles.Length);

            foreach (var testTextFile in testTextFiles)
            {
                var text = File.ReadAllText(testTextFile, Encoding.UTF8).Replace("\r\n", "\n");

                Assert.IsFalse(string.IsNullOrEmpty(text));
                Assert.IsTrue(await server.SendMessageAll(text));

                var messageTask = Task.Run(async () =>
                {
                    while (text != clientA.LastRawMessageReceived)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                    }
                });

                _ = await Task.WhenAny(messageTask, Task.Delay(TimeSpan.FromSeconds(5)));

                Assert.AreEqual(text, clientA.LastRawMessageReceived);
            }

            Assert.IsTrue(await server.Stop());
        }
    }
}
