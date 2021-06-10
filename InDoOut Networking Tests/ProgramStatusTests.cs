using InDoOut_Core.Entities.Programs;
using InDoOut_Core_Tests;
using InDoOut_Networking.Shared.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace InDoOut_Networking_Tests
{
    [TestClass]
    public class ProgramStatusTests
    {
        [TestMethod]
        public void StatusFromProgram()
        {
            var program = new Program();
            var function1 = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));
            var function2 = new TestFunction(() => Thread.Sleep(TimeSpan.FromSeconds(1)));

            Assert.IsTrue(program.AddFunction(function1));
            Assert.IsTrue(program.AddFunction(function2));

            var status = ProgramStatus.FromProgram(program);

            Assert.AreEqual(program.Id, status.Id);
            Assert.AreEqual(program.Functions.Count, status.Functions.Length);
            Assert.AreEqual(program.Name, status.Name);
            Assert.AreEqual(program.LastCompletionTime, status.LastCompletionTime);
            Assert.AreEqual(program.LastTriggerTime, status.LastTriggerTime);
            Assert.AreEqual(0, status.Metadata.Count);
            Assert.IsFalse(status.Stopping);
            Assert.IsFalse(status.Finishing);
            Assert.IsFalse(status.Running);

            var currentStatusFunction = status.Functions[0];
            var currentProgramFunction = program.Functions[0];
            Assert.AreEqual(currentProgramFunction.Id, currentStatusFunction.Id);
            Assert.AreEqual(currentProgramFunction.LastCompletionTime, currentStatusFunction.LastCompletionTime);
            Assert.AreEqual(currentProgramFunction.LastTriggerTime, currentStatusFunction.LastTriggerTime);
            Assert.AreEqual(currentProgramFunction.State, currentStatusFunction.State);

            currentStatusFunction = status.Functions[1];
            currentProgramFunction = program.Functions[1];
            Assert.AreEqual(currentProgramFunction.Id, currentStatusFunction.Id);
            Assert.AreEqual(currentProgramFunction.LastCompletionTime, currentStatusFunction.LastCompletionTime);
            Assert.AreEqual(currentProgramFunction.LastTriggerTime, currentStatusFunction.LastTriggerTime);
            Assert.AreEqual(currentProgramFunction.State, currentStatusFunction.State);

            function1.Trigger(null);

            Thread.Sleep(TimeSpan.FromMilliseconds(20));

            status = ProgramStatus.FromProgram(program);

            Assert.AreEqual(program.Id, status.Id);
            Assert.AreEqual(program.Functions.Count, status.Functions.Length);
            Assert.AreEqual(program.Name, status.Name);
            Assert.AreEqual(program.LastCompletionTime, status.LastCompletionTime);
            Assert.AreEqual(program.LastTriggerTime, status.LastTriggerTime);
            Assert.AreEqual(0, status.Metadata.Count);
            Assert.IsFalse(status.Stopping);
            Assert.IsFalse(status.Finishing);
            Assert.IsTrue(status.Running);

            currentStatusFunction = status.Functions[0];
            currentProgramFunction = program.Functions[0];
            Assert.AreEqual(currentProgramFunction.Id, currentStatusFunction.Id);
            Assert.AreEqual(currentProgramFunction.LastCompletionTime, currentStatusFunction.LastCompletionTime);
            Assert.AreEqual(currentProgramFunction.LastTriggerTime, currentStatusFunction.LastTriggerTime);
            Assert.AreEqual(currentProgramFunction.State, currentStatusFunction.State);

            currentStatusFunction = status.Functions[1];
            currentProgramFunction = program.Functions[1];
            Assert.AreEqual(currentProgramFunction.Id, currentStatusFunction.Id);
            Assert.AreEqual(currentProgramFunction.LastCompletionTime, currentStatusFunction.LastCompletionTime);
            Assert.AreEqual(currentProgramFunction.LastTriggerTime, currentStatusFunction.LastTriggerTime);
            Assert.AreEqual(currentProgramFunction.State, currentStatusFunction.State);

            program.Metadata.Add("test key", "test string");

            status = ProgramStatus.FromProgram(program);

            Assert.AreEqual(1, status.Metadata.Count);
            Assert.AreEqual("test string", status.Metadata["test key"]);

            program.Metadata.Add("Another key", "Another string");

            Assert.AreEqual(1, status.Metadata.Count);
            Assert.AreEqual("test string", status.Metadata["test key"]);

            status = ProgramStatus.FromProgram(program);

            Assert.AreEqual(2, status.Metadata.Count);
            Assert.AreEqual("test string", status.Metadata["test key"]);
            Assert.AreEqual("Another string", status.Metadata["Another key"]);
        }
    }
}
