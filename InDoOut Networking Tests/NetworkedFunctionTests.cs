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

            status.Inputs = new InputStatus[]
            {
                new InputStatus()
                {
                    Finishing = true,
                    Id = Guid.NewGuid(),
                    LastCompletionTime = DateTime.Today.AddHours(1),
                    LastTriggerTime = DateTime.Today.AddSeconds(-70),
                    Name = "A fresh new input!",
                    Running = false
                },
                new InputStatus()
                {
                    Finishing = false,
                    Id = input.Id,
                    LastCompletionTime = DateTime.Today.AddDays(10),
                    LastTriggerTime = DateTime.Today.AddMilliseconds(100),
                    Name = "Changing the name of the input",
                    Running = true
                }
            };
            status.Outputs = new OutputStatus[]
            {
                new OutputStatus()
                {
                    Finishing = false,
                    Id = Guid.NewGuid(),
                    LastCompletionTime = DateTime.MaxValue,
                    LastTriggerTime = DateTime.MinValue,
                    Name = "Testing output",
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
            Assert.AreEqual(2, networkedFunction.Inputs.Count);
            Assert.AreEqual(1, networkedFunction.Outputs.Count);
            Assert.AreEqual(0, networkedFunction.Properties.Count);
            Assert.AreEqual(0, networkedFunction.Results.Count);
            Assert.AreEqual(State.Completing, networkedFunction.State);

            input = networkedFunction.Inputs[0];

            Assert.IsFalse(input.Finishing);
            Assert.IsTrue(input.Running);

            Assert.AreEqual("Changing the name of the input", input.Name);
            Assert.AreEqual(status.Inputs[1].Id, input.Id);
            Assert.AreEqual(DateTime.Today.AddDays(10), input.LastCompletionTime);
            Assert.AreEqual(DateTime.Today.AddMilliseconds(100), input.LastTriggerTime);

            input = networkedFunction.Inputs[1];

            Assert.IsTrue(input.Finishing);
            Assert.IsFalse(input.Running);

            Assert.AreEqual("A fresh new input!", input.Name);
            Assert.AreEqual(status.Inputs[0].Id, input.Id);
            Assert.AreEqual(DateTime.Today.AddHours(1), input.LastCompletionTime);
            Assert.AreEqual(DateTime.Today.AddSeconds(-70), input.LastTriggerTime);
        }
    }
}
