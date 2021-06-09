using InDoOut_Core.Entities.Functions;
using InDoOut_Networking.Entities;
using InDoOut_Networking.Shared.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class NetworkedFunctionTests
    {
        [TestMethod]
        public void UpdateFunctionFromStatus()
        {
            var networkedFunction = new NetworkedFunction();

            Assert.IsNull(networkedFunction.Name);
            Assert.IsNull(networkedFunction.TriggerOnFailure);

            Assert.IsFalse(networkedFunction.Finishing);
            Assert.IsFalse(networkedFunction.Running);
            Assert.IsFalse(networkedFunction.StopRequested);

            Assert.AreNotEqual(Guid.Empty, networkedFunction.Id);
            Assert.AreEqual(DateTime.MinValue, networkedFunction.LastCompletionTime);
            Assert.AreEqual(DateTime.MinValue, networkedFunction.LastTriggerTime);
            Assert.AreEqual(0, networkedFunction.Inputs.Count);
            Assert.AreEqual(0, networkedFunction.Outputs.Count);
            Assert.AreEqual(0, networkedFunction.Properties.Count);
            Assert.AreEqual(0, networkedFunction.Results.Count);
            Assert.AreEqual(State.Unknown, networkedFunction.State);

            var status = new FunctionStatus()
            {
                State = State.Processing,
                Id = Guid.NewGuid(),
                LastCompletionTime = DateTime.Today.AddDays(-2),
                LastTriggerTime = DateTime.Today.AddMonths(-1),
                Name = "Test function"
            };

            Assert.IsFalse(networkedFunction.UpdateFromStatus(null));
            Assert.IsTrue(networkedFunction.UpdateFromStatus(status));

            Assert.IsNull(networkedFunction.TriggerOnFailure);

            Assert.IsFalse(networkedFunction.Finishing);
            Assert.IsFalse(networkedFunction.StopRequested);
            Assert.IsTrue(networkedFunction.Running);

            Assert.AreEqual(status.Id, networkedFunction.Id);
            Assert.AreEqual("Test function", networkedFunction.Name);
            Assert.AreEqual(DateTime.Today.AddDays(-2), networkedFunction.LastCompletionTime);
            Assert.AreEqual(DateTime.Today.AddMonths(-1), networkedFunction.LastTriggerTime);
            Assert.AreEqual(0, networkedFunction.Inputs.Count);
            Assert.AreEqual(0, networkedFunction.Outputs.Count);
            Assert.AreEqual(0, networkedFunction.Properties.Count);
            Assert.AreEqual(0, networkedFunction.Results.Count);
            Assert.AreEqual(State.Processing, networkedFunction.State);

            status.Name = "The name has changed";
            status.State = State.Completing;
            status.LastTriggerTime = DateTime.Today.AddSeconds(-100);

            Assert.IsTrue(networkedFunction.UpdateFromStatus(status));

            Assert.IsNull(networkedFunction.TriggerOnFailure);

            Assert.IsTrue(networkedFunction.Finishing);
            Assert.IsFalse(networkedFunction.StopRequested);
            Assert.IsFalse(networkedFunction.Running);

            Assert.AreEqual(status.Id, networkedFunction.Id);
            Assert.AreEqual("The name has changed", networkedFunction.Name);
            Assert.AreEqual(DateTime.Today.AddDays(-2), networkedFunction.LastCompletionTime);
            Assert.AreEqual(DateTime.Today.AddSeconds(-100), networkedFunction.LastTriggerTime);
            Assert.AreEqual(0, networkedFunction.Inputs.Count);
            Assert.AreEqual(0, networkedFunction.Outputs.Count);
            Assert.AreEqual(0, networkedFunction.Properties.Count);
            Assert.AreEqual(0, networkedFunction.Results.Count);
            Assert.AreEqual(State.Completing, networkedFunction.State);

            status.Name = "And it changed again";
            status.Inputs = new InputStatus[]
            {
                new InputStatus()
                {
                    Finishing = false,
                    Id = Guid.NewGuid(),
                    LastCompletionTime = DateTime.MinValue,
                    LastTriggerTime = DateTime.MinValue,
                    Name = "Testing input",
                    Running = false
                }
            };

            Assert.IsTrue(networkedFunction.UpdateFromStatus(status));

            Assert.IsNull(networkedFunction.TriggerOnFailure);

            Assert.IsTrue(networkedFunction.Finishing);
            Assert.IsFalse(networkedFunction.StopRequested);
            Assert.IsFalse(networkedFunction.Running);

            Assert.AreEqual(status.Id, networkedFunction.Id);
            Assert.AreEqual("And it changed again", networkedFunction.Name);
            Assert.AreEqual(DateTime.Today.AddDays(-2), networkedFunction.LastCompletionTime);
            Assert.AreEqual(DateTime.Today.AddSeconds(-100), networkedFunction.LastTriggerTime);
            Assert.AreEqual(1, networkedFunction.Inputs.Count);
            Assert.AreEqual(0, networkedFunction.Outputs.Count);
            Assert.AreEqual(0, networkedFunction.Properties.Count);
            Assert.AreEqual(0, networkedFunction.Results.Count);
            Assert.AreEqual(State.Completing, networkedFunction.State);

            var input = networkedFunction.Inputs[0];

            Assert.IsFalse(input.Finishing);
            Assert.IsFalse(input.Running);

            Assert.AreEqual("Testing input", input.Name);
            Assert.AreEqual(status.Inputs[0].Id, input.Id);
            Assert.AreEqual(DateTime.MinValue, input.LastCompletionTime);
            Assert.AreEqual(DateTime.MinValue, input.LastTriggerTime);
        }
    }
}
