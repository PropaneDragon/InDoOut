using InDoOut_Core.Entities.Functions;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class InteractiveEntityTests
    {
        [TestMethod]
        public void AddConnections()
        {
            var interactiveEntity = new TestInteractiveEntity();
            var input = new Input(null);
            var output = new OutputNeutral();
            var function = new TestFunction(() => { });
            var basicTriggerable = new TestTriggerableInterface();

            Assert.AreEqual(0, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(0, interactiveEntity.Connections.Count);
            Assert.IsTrue(interactiveEntity.AddConnectionPublic(basicTriggerable));
            Assert.AreEqual(1, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(1, interactiveEntity.Connections.Count);
            Assert.IsFalse(interactiveEntity.AddConnectionPublic(basicTriggerable));
            Assert.AreEqual(1, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(1, interactiveEntity.Connections.Count);

            Assert.IsTrue(interactiveEntity.RemoveAllConnectionsPublic());
            Assert.AreEqual(0, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(0, interactiveEntity.Connections.Count);
            Assert.IsTrue(interactiveEntity.AddConnectionPublic(basicTriggerable));
            Assert.AreEqual(1, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(1, interactiveEntity.Connections.Count);

            Assert.IsTrue(interactiveEntity.AddConnectionPublic(input));
            Assert.AreEqual(2, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(2, interactiveEntity.Connections.Count);
            Assert.IsTrue(interactiveEntity.AddConnectionPublic(output));
            Assert.AreEqual(3, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(3, interactiveEntity.Connections.Count);
            Assert.IsTrue(interactiveEntity.AddConnectionPublic(function));
            Assert.AreEqual(4, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(4, interactiveEntity.Connections.Count);

            Assert.IsTrue(interactiveEntity.RemoveConnectionPublic(basicTriggerable));
            Assert.AreEqual(3, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(3, interactiveEntity.Connections.Count);
            Assert.IsFalse(interactiveEntity.RemoveConnectionPublic(basicTriggerable));
            Assert.AreEqual(3, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(3, interactiveEntity.Connections.Count);
            Assert.IsTrue(interactiveEntity.AddConnectionPublic(basicTriggerable));
            Assert.AreEqual(4, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(4, interactiveEntity.Connections.Count);

            Assert.IsTrue(interactiveEntity.RemoveConnectionsPublic(basicTriggerable, input, output));
            Assert.AreEqual(1, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(1, interactiveEntity.Connections.Count);
            Assert.AreEqual(function, interactiveEntity.RawConnections.First());
            Assert.AreEqual(function, interactiveEntity.Connections.First());
            Assert.IsFalse(interactiveEntity.RemoveConnectionsPublic(basicTriggerable, input, output));
            Assert.AreEqual(1, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(1, interactiveEntity.Connections.Count);
            Assert.AreEqual(function, interactiveEntity.RawConnections.First());
            Assert.AreEqual(function, interactiveEntity.Connections.First());
            Assert.IsFalse(interactiveEntity.RemoveConnectionsPublic(basicTriggerable, input, output, function));
            Assert.AreEqual(0, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(0, interactiveEntity.Connections.Count);

            Assert.IsFalse(interactiveEntity.AddConnectionsPublic(function, function, function, input));
            Assert.AreEqual(2, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(2, interactiveEntity.Connections.Count);
            Assert.IsTrue(interactiveEntity.RemoveAllConnectionsPublic());

            Assert.IsTrue(interactiveEntity.AddConnectionsPublic(basicTriggerable, input, output, function));
            Assert.AreEqual(4, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(4, interactiveEntity.Connections.Count);

            Assert.IsFalse(interactiveEntity.AddConnectionPublic(interactiveEntity));
            Assert.AreEqual(4, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(4, interactiveEntity.Connections.Count);
            Assert.IsFalse(interactiveEntity.AddConnectionsPublic(interactiveEntity, interactiveEntity, input));
            Assert.AreEqual(4, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(4, interactiveEntity.Connections.Count);

            Assert.IsFalse(interactiveEntity.SetConnectionPublic(interactiveEntity));
            Assert.AreEqual(0, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(0, interactiveEntity.Connections.Count);

            Assert.IsTrue(interactiveEntity.AddConnectionsPublic(basicTriggerable, input, output, function));
            Assert.AreEqual(4, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(4, interactiveEntity.Connections.Count);
            Assert.IsTrue(interactiveEntity.SetConnectionPublic(basicTriggerable, output));
            Assert.AreEqual(2, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(2, interactiveEntity.Connections.Count);
            Assert.AreEqual(basicTriggerable, interactiveEntity.RawConnections[0]);
            Assert.AreEqual(basicTriggerable, interactiveEntity.Connections[0]);
            Assert.AreEqual(output, interactiveEntity.RawConnections[1]);
            Assert.AreEqual(output, interactiveEntity.Connections[1]);

            Assert.IsTrue(interactiveEntity.RemoveAllConnectionsPublic());
            Assert.IsTrue(interactiveEntity.RemoveAllConnectionsPublic());
            Assert.IsTrue(interactiveEntity.SetConnectionPublic(function, input));
            Assert.AreEqual(2, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(2, interactiveEntity.Connections.Count);
            Assert.AreEqual(function, interactiveEntity.RawConnections[0]);
            Assert.AreEqual(function, interactiveEntity.Connections[0]);
            Assert.AreEqual(input, interactiveEntity.RawConnections[1]);
            Assert.AreEqual(input, interactiveEntity.Connections[1]);
            Assert.IsTrue(interactiveEntity.SetConnectionPublic(basicTriggerable, output));
            Assert.AreEqual(2, interactiveEntity.RawConnections.Count);
            Assert.AreEqual(2, interactiveEntity.Connections.Count);
            Assert.AreEqual(basicTriggerable, interactiveEntity.RawConnections[0]);
            Assert.AreEqual(basicTriggerable, interactiveEntity.Connections[0]);
            Assert.AreEqual(output, interactiveEntity.RawConnections[1]);
            Assert.AreEqual(output, interactiveEntity.Connections[1]);
        }

        [TestMethod]
        public void Trigger()
        {
            var interactiveEntity = new TestInteractiveEntity();
            var otherInteractiveEntity = new TestInteractiveEntity();

            Assert.IsFalse(interactiveEntity.Processed);

            interactiveEntity.Trigger(otherInteractiveEntity);

            var startTime = DateTime.Now;

            while (!interactiveEntity.Processed && DateTime.Now < startTime.AddSeconds(1))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(interactiveEntity.Processed);
            Assert.AreEqual(otherInteractiveEntity, interactiveEntity.LastTriggeredBy);

            interactiveEntity.Processed = false;

            Assert.IsFalse(interactiveEntity.Processed);

            Thread.Sleep(TimeSpan.FromMilliseconds(10));

            interactiveEntity.Trigger(null);

            startTime = DateTime.Now;

            while (!interactiveEntity.Processed && DateTime.Now < startTime.AddSeconds(1))
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(1));
            }

            Assert.IsTrue(interactiveEntity.Processed);
            Assert.IsNull(interactiveEntity.LastTriggeredBy);
        }

        [TestMethod]
        public void TriggeredWithin()
        {
            var interactiveEntity = new TestInteractiveEntity(() => Thread.Sleep(TimeSpan.FromMilliseconds(15)));

            Assert.IsFalse(interactiveEntity.HasBeenTriggeredSince(DateTime.Now));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredSince(DateTime.Today));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredSince(DateTime.MinValue.AddSeconds(1)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredSince(DateTime.MinValue));

            Assert.IsFalse(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromMilliseconds(15)));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromSeconds(1)));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromSeconds(100)));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromDays(1)));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromDays(1000)));

            interactiveEntity.Trigger(null);

            Assert.IsTrue(interactiveEntity.WaitForCompletion(TimeSpan.FromMilliseconds(30), false));

            Assert.IsFalse(interactiveEntity.HasBeenTriggeredSince(DateTime.Now));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredSince(DateTime.Now - TimeSpan.FromMilliseconds(10)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredSince(DateTime.Now - TimeSpan.FromMilliseconds(20)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredSince(DateTime.Now - TimeSpan.FromSeconds(1)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredSince(DateTime.Now - TimeSpan.FromDays(1)));

            Assert.IsFalse(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromMilliseconds(1)));
            Assert.IsFalse(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromMilliseconds(10)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromMilliseconds(20)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromMilliseconds(200)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromSeconds(20)));
            Assert.IsTrue(interactiveEntity.HasBeenTriggeredWithin(TimeSpan.FromDays(20)));
        }
    }
}
