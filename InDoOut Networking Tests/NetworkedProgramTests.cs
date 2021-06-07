using InDoOut_Core_Tests;
using InDoOut_Networking.Shared.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class NetworkedProgramTests
    {
        [TestMethod]
        public void UpdateFromStatus()
        {
            var client = new TestClient();
            var networkedProgram = new TestNetworkedProgram(client);
            var status = new ProgramStatus()
            {
                Running = true,
                Id = Guid.NewGuid(),
                Finishing = false,
                LastCompletionTime = DateTime.Today.AddDays(1),
                LastTriggerTime = DateTime.Today,
                Name = "This is a test",
                Stopping = false
            };

            Assert.IsFalse(networkedProgram.Running);
            Assert.IsFalse(networkedProgram.Finishing);
            Assert.IsFalse(networkedProgram.Stopping);

            Assert.AreNotEqual(networkedProgram.Id, status.Id);
            Assert.AreNotEqual(networkedProgram.LastCompletionTime, status.LastCompletionTime);
            Assert.AreNotEqual(networkedProgram.LastTriggerTime, status.LastTriggerTime);
            Assert.AreNotEqual(networkedProgram.Name, status.Name);

            Assert.IsFalse(networkedProgram.UpdateFromStatus(null));

            Assert.IsFalse(networkedProgram.Running);
            Assert.IsFalse(networkedProgram.Finishing);
            Assert.IsFalse(networkedProgram.Stopping);

            Assert.AreNotEqual(networkedProgram.Id, status.Id);
            Assert.AreNotEqual(networkedProgram.LastCompletionTime, status.LastCompletionTime);
            Assert.AreNotEqual(networkedProgram.LastTriggerTime, status.LastTriggerTime);
            Assert.AreNotEqual(networkedProgram.Name, status.Name);

            Assert.IsTrue(networkedProgram.UpdateFromStatus(status));

            Assert.IsTrue(networkedProgram.Running);
            Assert.IsFalse(networkedProgram.Finishing);
            Assert.IsFalse(networkedProgram.Stopping);

            Assert.AreEqual(networkedProgram.Id, status.Id);
            Assert.AreEqual(networkedProgram.LastCompletionTime, status.LastCompletionTime);
            Assert.AreEqual(networkedProgram.LastTriggerTime, status.LastTriggerTime);
            Assert.AreEqual(networkedProgram.Name, $"{status.Name} [Disconnected]");

            status.Running = false;
            status.Finishing = true;
            status.Name = "The name has changed";

            Assert.IsTrue(networkedProgram.UpdateFromStatus(status));

            Assert.IsFalse(networkedProgram.Running);
            Assert.IsTrue(networkedProgram.Finishing);
            Assert.IsFalse(networkedProgram.Stopping);

            Assert.AreEqual(networkedProgram.Id, status.Id);
            Assert.AreEqual(networkedProgram.LastCompletionTime, status.LastCompletionTime);
            Assert.AreEqual(networkedProgram.LastTriggerTime, status.LastTriggerTime);
            Assert.AreEqual(networkedProgram.Name, $"{status.Name} [Disconnected]");
        }

        [TestMethod]
        public void UpdateFromStatusWithFunction()
        {
            var function1 = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var function2 = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var client = new TestClient();
            var networkedProgram = new TestNetworkedProgram(client);
            var status = new ProgramStatus()
            {
                Running = true,
                Id = Guid.NewGuid(),
                Finishing = false,
                LastCompletionTime = DateTime.Today.AddDays(1),
                LastTriggerTime = DateTime.Today,
                Name = "This is a test",
                Stopping = false
            };

            /*Assert.AreEqual(2, networkedProgram.NetworkedFunctions.Count);

            var currentFunction = networkedProgram.NetworkedFunctions[0];
            Assert.AreEqual(function1.Id, currentFunction.Id);
            Assert.IsFalse(currentFunction.Running);
            Assert.AreEqual(State.Unknown, currentFunction.State);
            Assert.AreEqual(DateTime.MinValue, currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.MinValue, currentFunction.LastCompletionTime);

            currentFunction = networkedProgram.NetworkedFunctions[1];
            Assert.AreEqual(function2.Id, currentFunction.Id);
            Assert.IsFalse(currentFunction.Running);
            Assert.AreEqual(State.Unknown, currentFunction.State);
            Assert.AreEqual(DateTime.MinValue, currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.MinValue, currentFunction.LastCompletionTime);

            var function1Status = new FunctionStatus() { Id = function1.Id, LastCompletionTime = DateTime.Today.AddDays(-1), LastTriggerTime = DateTime.Today.AddDays(-2), State = State.InError };
            var function2Status = new FunctionStatus() { Id = function2.Id, LastCompletionTime = DateTime.Today, LastTriggerTime = DateTime.Today, State = State.Processing };

            status.Id = program.Id;
            status.Name = program.Name;
            status.Functions = new FunctionStatus[]
            {
                new FunctionStatus() { Id = Guid.NewGuid(), LastCompletionTime = DateTime.Now, LastTriggerTime = DateTime.Now, State = State.Unknown },
                function2Status
            };

            Assert.IsTrue(networkedProgram.UpdateFromProgramStatusPublic(status));

            currentFunction = networkedProgram.NetworkedFunctions[0];
            Assert.AreEqual(function1.Id, currentFunction.Id);
            Assert.IsFalse(currentFunction.Running);
            Assert.AreEqual(State.Unknown, currentFunction.State);
            Assert.AreEqual(DateTime.MinValue, currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.MinValue, currentFunction.LastCompletionTime);

            currentFunction = networkedProgram.NetworkedFunctions[1];
            Assert.AreEqual(function2.Id, currentFunction.Id);
            Assert.IsTrue(currentFunction.Running);
            Assert.AreEqual(State.Processing, currentFunction.State);
            Assert.AreEqual(DateTime.Today, currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.Today, currentFunction.LastCompletionTime);

            status.Functions = new FunctionStatus[]
            {
                function1Status,
                function2Status
            };

            Assert.IsTrue(networkedProgram.UpdateFromProgramStatusPublic(status));

            currentFunction = networkedProgram.NetworkedFunctions[0];
            Assert.AreEqual(function1.Id, currentFunction.Id);
            Assert.IsFalse(currentFunction.Running);
            Assert.AreEqual(State.InError, currentFunction.State);
            Assert.AreEqual(DateTime.Today.AddDays(-2), currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.Today.AddDays(-1), currentFunction.LastCompletionTime);

            currentFunction = networkedProgram.NetworkedFunctions[1];
            Assert.AreEqual(function2.Id, currentFunction.Id);
            Assert.IsTrue(currentFunction.Running);
            Assert.AreEqual(State.Processing, currentFunction.State);
            Assert.AreEqual(DateTime.Today, currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.Today, currentFunction.LastCompletionTime);

            function1Status.State = State.Waiting;

            status.Functions = new FunctionStatus[]
            {
                function1Status,
                function2Status
            };

            Assert.IsTrue(networkedProgram.UpdateFromProgramStatusPublic(status));

            currentFunction = networkedProgram.NetworkedFunctions[0];
            Assert.AreEqual(function1.Id, currentFunction.Id);
            Assert.IsFalse(currentFunction.Running);
            Assert.AreEqual(State.Waiting, currentFunction.State);
            Assert.AreEqual(DateTime.Today.AddDays(-2), currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.Today.AddDays(-1), currentFunction.LastCompletionTime);

            currentFunction = networkedProgram.NetworkedFunctions[1];
            Assert.AreEqual(function2.Id, currentFunction.Id);
            Assert.IsTrue(currentFunction.Running);
            Assert.AreEqual(State.Processing, currentFunction.State);
            Assert.AreEqual(DateTime.Today, currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.Today, currentFunction.LastCompletionTime);

            function1Status.State = State.Processing;
            function2Status.State = State.Completing;

            status.Functions = new FunctionStatus[]
            {
                function1Status,
                function2Status
            };

            Assert.IsTrue(networkedProgram.UpdateFromProgramStatusPublic(status));

            currentFunction = networkedProgram.NetworkedFunctions[0];
            Assert.AreEqual(function1.Id, currentFunction.Id);
            Assert.IsTrue(currentFunction.Running);
            Assert.AreEqual(State.Processing, currentFunction.State);
            Assert.AreEqual(DateTime.Today.AddDays(-2), currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.Today.AddDays(-1), currentFunction.LastCompletionTime);

            currentFunction = networkedProgram.NetworkedFunctions[1];
            Assert.AreEqual(function2.Id, currentFunction.Id);
            Assert.IsFalse(currentFunction.Running);
            Assert.AreEqual(State.Completing, currentFunction.State);
            Assert.AreEqual(DateTime.Today, currentFunction.LastTriggerTime);
            Assert.AreEqual(DateTime.Today, currentFunction.LastCompletionTime);*/ //Todo
        }
    }
}
