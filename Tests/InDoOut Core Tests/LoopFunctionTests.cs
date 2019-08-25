using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class LoopFunctionTests
    {
        [TestMethod]
        public void LoopCount()
        {
            var function = new TestLoopFunction();
            var inputFirstItem = function.GetInputByName("First item");
            var inputNextItem = function.GetInputByName("Next item");

            Assert.IsNotNull(inputFirstItem);
            Assert.IsNotNull(inputNextItem);

            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);
            Assert.AreEqual(5, function.ItemsToIterate);
            Assert.AreEqual("0", function.GetResultValue("Current index"));

            function.Trigger(inputFirstItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("0", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsTrue(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputFirstItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("0", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsTrue(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputNextItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("1", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputNextItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("2", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputNextItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("3", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputNextItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("4", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputNextItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("4", function.GetResultValue("Current index"));
            Assert.IsTrue(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputFirstItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("0", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsTrue(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputNextItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("1", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputNextItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("2", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsFalse(function.PreprocessCalled);

            function.AllItemsCompleteCalled = false;
            function.PreprocessCalled = false;

            function.Trigger(inputFirstItem);
            function.WaitForCompletion(true);

            Assert.AreEqual("0", function.GetResultValue("Current index"));
            Assert.IsFalse(function.AllItemsCompleteCalled);
            Assert.IsTrue(function.PreprocessCalled);
        }
    }
}
