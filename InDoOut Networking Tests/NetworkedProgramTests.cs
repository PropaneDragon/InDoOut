using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Core_Tests;
using InDoOut_Networking.Entities;
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

            var function1 = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var function2 = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));

            var program = new Program();

            Assert.IsTrue(program.AddFunction(function1));
            Assert.IsTrue(program.AddFunction(function2));

            networkedProgram.AssociatedProgram = program;

            Assert.AreEqual(2, networkedProgram.NetworkedFunctions.Count);

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
            Assert.AreEqual(DateTime.Today, currentFunction.LastCompletionTime);
        }
    }
}
