using InDoOut_Desktop.Loading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InDoOut_Desktop_Tests
{
    [TestClass]
    public class LoadingTaskTests
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        [TestMethod]
        public async Task Run()
        {
            var hasRun = false;
            var task = new TestLoadingTask("test task", async (task) =>
            {
                hasRun = true;
                return hasRun;
            });

            Assert.AreEqual("test task", task.Name);
            Assert.AreEqual(TaskState.NotRun, task.State);
            Assert.IsFalse(hasRun);
            Assert.IsTrue(await task.RunAsync());
            Assert.IsTrue(hasRun);
            Assert.AreEqual(TaskState.HasRun, task.State);

            hasRun = false;
            task = new TestLoadingTask("test failure", async (task) =>
            {
                hasRun = true;
                return false;
            });

            Assert.AreEqual("test failure", task.Name);
            Assert.AreEqual(TaskState.NotRun, task.State);
            Assert.IsFalse(hasRun);
            Assert.IsFalse(await task.RunAsync());
            Assert.IsTrue(hasRun);
            Assert.AreEqual(TaskState.HasRun, task.State);
        }

        [TestMethod]
        public async Task RunChildren()
        {
            var hasRun = false;
            var childrenRun = 0;
            var runOrder = new List<string>();
            var mainTask = new TestLoadingTask("parent task", async (task) =>
            {
                runOrder.Add(task.Name);
                hasRun = true;
                return hasRun;
            });

            var childTaskA = new TestLoadingTask("child task A", async (task) => { ++childrenRun; runOrder.Add(task.Name); return true; });
            var childTaskB = new TestLoadingTask("child task B", async (task) => { ++childrenRun; runOrder.Add(task.Name); return true; });
            var childTaskC = new TestLoadingTask("child task C", async (task) => { ++childrenRun; runOrder.Add(task.Name); return true; });
            var childTaskD = new TestLoadingTask("child task D", async (task) => { ++childrenRun; runOrder.Add(task.Name); return true; });

            Assert.IsTrue(childTaskA.Add(childTaskB));
            Assert.IsTrue(childTaskB.Add(childTaskD));
            Assert.IsTrue(childTaskA.Add(childTaskC));
            Assert.IsTrue(mainTask.Add(childTaskA));

            Assert.IsTrue(await mainTask.RunAsync());

            Assert.AreEqual(4, childrenRun);
            Assert.AreEqual(5, runOrder.Count);
            Assert.AreEqual("child task D", runOrder[0]);
            Assert.AreEqual("child task B", runOrder[1]);
            Assert.AreEqual("child task C", runOrder[2]);
            Assert.AreEqual("child task A", runOrder[3]);
            Assert.AreEqual("parent task", runOrder[4]);
        }

        [TestMethod]
        public async Task PropagatedFailures()
        {
            var childTaskA = new TestLoadingTask("child task A", async (task) => true);
            var childTaskB = new TestLoadingTask("child task B", async (task) => true);
            var childTaskC = new TestLoadingTask("child task C", async (task) => true);
            var childTaskD = new TestLoadingTask("child task D", async (task) => false);

            Assert.IsTrue(childTaskA.Add(childTaskB));
            Assert.IsTrue(childTaskB.Add(childTaskD));
            Assert.IsTrue(childTaskA.Add(childTaskC));

            Assert.IsFalse(await childTaskD.RunAsync());
            Assert.IsFalse(await childTaskB.RunAsync());
            Assert.IsFalse(await childTaskA.RunAsync());
            Assert.IsTrue(await childTaskC.RunAsync());
        }

        [TestMethod]
        public async Task StopOnFailure()
        {
            var tasksRun = new List<ILoadingTask>();
            var childTaskA = new TestLoadingTask("child task A", async (task) => { tasksRun.Add(task); return true; });
            var childTaskB = new TestLoadingTask("child task B", async (task) => { tasksRun.Add(task); return true; });
            var childTaskC = new TestLoadingTask("child task C", async (task) => { tasksRun.Add(task); return false; });
            var childTaskD = new TestLoadingTask("child task D", async (task) => { tasksRun.Add(task); return true; });

            Assert.IsTrue(childTaskA.Add(childTaskB));
            Assert.IsTrue(childTaskB.Add(childTaskC));
            Assert.IsTrue(childTaskC.Add(childTaskD));

            Assert.IsFalse(await childTaskA.RunAsync());

            Assert.AreEqual(2, tasksRun.Count);
            Assert.AreEqual(childTaskD, tasksRun[0]);
            Assert.AreEqual(childTaskC, tasksRun[1]);
        }

        [TestMethod]
        public async Task NameChangeEvent()
        {
            var timesChanged = 0;
            var task = new TestLoadingTask("test task", async (task) =>
            {
                task.Name = "Test 1";
                task.Name = "Test 2";
                task.Name = "Test 3";
                return true;
            });

            task.NameChanged += (sender, e) => ++timesChanged;

            Assert.IsTrue(await task.RunAsync());
            Assert.AreEqual(3, timesChanged);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
