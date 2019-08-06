using InDoOut_Desktop_API_Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InDoOut_Plugins_Tests
{
    [TestClass]
    public class PluginTests
    {
        [TestMethod]
        public void Values()
        {
            var plugin = new TestPlugin()
            {
                PublicAuthor = "An author",
                PublicDescription = "A description of the plugin",
                PublicName = "A name"
            };

            Assert.AreEqual("An author", plugin.Author);
            Assert.AreEqual("An author", plugin.SafeAuthor);

            Assert.AreEqual("A description of the plugin", plugin.Description);
            Assert.AreEqual("A description of the plugin", plugin.SafeDescription);

            Assert.AreEqual("A name", plugin.Name);
            Assert.AreEqual("A name", plugin.SafeName);
        }

        [TestMethod]
        public void Safety()
        {
            var plugin = new TestExceptionPlugin();

            _ = Assert.ThrowsException<Exception>(() => plugin.Name);
            _ = Assert.ThrowsException<Exception>(() => plugin.Description);
            _ = Assert.ThrowsException<Exception>(() => plugin.Author);

            Assert.AreEqual("Unknown name", plugin.SafeName);
            Assert.AreEqual("", plugin.SafeDescription);
            Assert.AreEqual("Unknown author", plugin.SafeAuthor);
        }
    }
}
