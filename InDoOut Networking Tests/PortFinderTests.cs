using InDoOut_Networking.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Sockets;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class PortFinderTests
    {
        [TestMethod]
        public void AnyOpenPort()
        {
            var openPort = PortFinder.Find();

            Assert.AreNotEqual(-1, openPort);

            var listener = new TcpListener(IPAddress.Loopback, openPort);
            listener.Start();

            Assert.AreEqual((listener.LocalEndpoint as IPEndPoint).Port, openPort);
            Assert.AreNotEqual(openPort, PortFinder.Find());

            var clashListener = new TcpListener(IPAddress.Loopback, openPort);

            _ = Assert.ThrowsException<SocketException>(() => clashListener.Start());

            listener.Stop();
            clashListener.Stop();
        }

        [TestMethod]
        public void AnyPortRange()
        {
            for (var count = 0; count < 20; ++count)
            {
                var openPort = PortFinder.Find();

                Assert.AreNotEqual(-1, openPort);

                var portEnd = openPort + 100;
                var foundPort = PortFinder.Find(openPort, portEnd);

                Assert.IsTrue(foundPort >= openPort);
                Assert.IsTrue(foundPort < portEnd);
            }
        }
    }
}
