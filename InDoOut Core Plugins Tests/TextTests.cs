using InDoOut_Core_Plugins.Text;
using InDoOut_Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Plugins_Tests
{
    [TestClass]
    public class TextTests
    {
        [TestMethod]
        public void CombineText()
        {
            var functionFullTest = new CombineTextFunction();

            Assert.IsTrue(functionFullTest.SetPropertyValue("String 1", "1 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 2", "2 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 3", "3 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 4", "4 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 5", "5 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 6", "6 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 7", "7 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 8", "8 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 9", "9 "));
            Assert.IsTrue(functionFullTest.SetPropertyValue("String 10", "10"));

            functionFullTest.Trigger(null);
            functionFullTest.WaitForCompletion(true);

            Assert.AreEqual("1 2 3 4 5 6 7 8 9 10", functionFullTest.GetResultValue("Combined text"));

            var functionPartTest = new CombineTextFunction();

            Assert.IsTrue(functionPartTest.SetPropertyValue("String 1", "This "));
            Assert.IsTrue(functionPartTest.SetPropertyValue("String 3", "is "));
            Assert.IsTrue(functionPartTest.SetPropertyValue("String 4", "a "));
            Assert.IsTrue(functionPartTest.SetPropertyValue("String 9", "test"));
            Assert.IsTrue(functionPartTest.SetPropertyValue("String 10", "!!!!!!"));

            functionPartTest.Trigger(null);
            functionPartTest.WaitForCompletion(true);

            Assert.AreEqual("This is a test!!!!!!", functionPartTest.GetResultValue("Combined text"));
        }
    }
}
