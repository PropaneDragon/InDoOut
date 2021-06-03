using InDoOut_Core.Entities.Programs;
using InDoOut_Networking.Entities;
using InDoOut_Networking.Shared.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class NetworkedProgramTests
    {
        [TestMethod]
        public void PopulateFromProgram()
        {
            var program = new Program();
            var function1 = new TestFunction() { Id = Guid.NewGuid() };
            var function2 = new TestFunction() { Id = Guid.NewGuid() };
            var client = new TestClient();
            var networkedProgram = new NetworkedProgram(client);

            Assert.AreNotEqual(program, networkedProgram.AssociatedProgram);
            Assert.AreNotEqual(program.Id, networkedProgram.Id);
            Assert.AreEqual(0, networkedProgram.Functions.Count);

            networkedProgram.AssociatedProgram = program;

            Assert.AreEqual(program, networkedProgram.AssociatedProgram);
            Assert.AreEqual(program.Id, networkedProgram.Id);
            Assert.AreEqual(0, networkedProgram.Functions.Count);

            _ = program.AddFunction(function1);

            Assert.AreEqual(1, networkedProgram.Functions.Count);
            Assert.AreEqual(function1.Id, networkedProgram.Functions[0].Id);

            _ = program.AddFunction(function2);

            Assert.AreEqual(2, networkedProgram.Functions.Count);
            Assert.AreEqual(function1.Id, networkedProgram.Functions[0].Id);
            Assert.AreEqual(function2.Id, networkedProgram.Functions[1].Id);
        }

        [TestMethod]
        public void UpdateFromStatus()
        {
            var client = new TestClient();
            var networkedProgram = new TestNetworkedProgram(client);
            var status = new ProgramStatus()
            {
                Id = Guid.NewGuid(),
                Name = networkedProgram.Name,
                Running = true
            };

            Assert.AreEqual(false, networkedProgram.Running);

            Assert.IsFalse(networkedProgram.UpdateFromProgramStatusPublic(null));
            Assert.IsFalse(networkedProgram.UpdateFromProgramStatusPublic(status));
            Assert.IsFalse(networkedProgram.Running);

            status.Id = networkedProgram.Id;

            Assert.IsTrue(networkedProgram.UpdateFromProgramStatusPublic(status));
            Assert.IsTrue(networkedProgram.Running);

            status.Running = false;

            Assert.IsTrue(networkedProgram.UpdateFromProgramStatusPublic(status));
            Assert.IsFalse(networkedProgram.Running);
        }
    }
}
