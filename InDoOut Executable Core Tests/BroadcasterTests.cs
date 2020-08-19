using InDoOut_Executable_Core.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InDoOut_Executable_Core_Tests
{
    [TestClass]
    public class BroadcasterTests
    {
        [TestMethod]
        public async Task BroadcasterSendReceive()
        {
            var broadcaster = new Broadcaster();

            Assert.IsTrue(await broadcaster.Begin(IPAddress.Loopback, 9000));
            Assert.IsTrue(broadcaster.Connected);

            var delayTask = Task.Delay(TimeSpan.FromSeconds(1));
            var receiveTask = broadcaster.Listen();

            Assert.IsTrue(await broadcaster.Send("This is a test of a long(ish) string that should successfully get sent using the broadcaster."));

            var completedTask = await Task.WhenAny(receiveTask, delayTask);

            Assert.AreEqual(receiveTask, completedTask);
            Assert.AreEqual("This is a test of a long(ish) string that should successfully get sent using the broadcaster.", receiveTask.Result);

            delayTask = Task.Delay(TimeSpan.FromSeconds(1));
            receiveTask = broadcaster.Listen();
            completedTask = await Task.WhenAny(receiveTask, delayTask);

            Assert.AreEqual(delayTask, completedTask);

            Assert.IsTrue(await broadcaster.Begin(IPAddress.Loopback, 9000));

            delayTask = Task.Delay(TimeSpan.FromSeconds(1));
            receiveTask = broadcaster.Listen();

            Assert.IsTrue(await broadcaster.Send("This is a different test to see if re-beginning the broadcaster still works."));

            completedTask = await Task.WhenAny(receiveTask, delayTask);

            Assert.AreEqual(receiveTask, completedTask);
            Assert.AreEqual("This is a different test to see if re-beginning the broadcaster still works.", receiveTask.Result);

            delayTask = Task.Delay(TimeSpan.FromSeconds(1));
            receiveTask = broadcaster.Listen();

            Assert.IsTrue(await broadcaster.Send("This is completely different."));

            completedTask = await Task.WhenAny(receiveTask, delayTask);

            Assert.AreEqual(receiveTask, completedTask);
            Assert.AreEqual("This is completely different.", receiveTask.Result);

            broadcaster.End();
        }

        [TestMethod]
        public async Task BroadcasterSendReceiveUTF8()
        {
            var broadcaster = new Broadcaster();
            Assert.IsTrue(await broadcaster.Begin(IPAddress.Broadcast, 9001));
            Assert.IsTrue(broadcaster.Connected);

            var testTextFiles = Directory.GetFiles(".", "*.txt");
            Assert.AreEqual(5, testTextFiles.Length);

            foreach (var testTextFile in testTextFiles)
            {
                var text = File.ReadAllText(testTextFile, Encoding.UTF8);

                Assert.IsFalse(string.IsNullOrEmpty(text));

                var delayTask = Task.Delay(TimeSpan.FromSeconds(1));
                var receiveTask = broadcaster.Listen();

                Assert.IsTrue(await broadcaster.Send(text));

                var completedTask = await Task.WhenAny(receiveTask, delayTask);

                Assert.AreEqual(receiveTask, completedTask);
                Assert.AreEqual(text, receiveTask.Result);
            }

            broadcaster.End();
        }
    }
}
