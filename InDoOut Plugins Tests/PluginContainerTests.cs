using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop_API_Tests;
using InDoOut_Desktop_API_Tests.External_Plugin_Testing;
using InDoOut_Plugins.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Plugins_Tests
{
    [TestClass]
    public class PluginContainerTests
    {
        [TestMethod]
        public void ImportTypes()
        {
            var plugin = new TestPlugin()
            {
                PublicAuthor = "None",
                PublicDescription = "Description",
                PublicName = "Name"
            };
            var pluginContainer = new PluginContainer(plugin);

            Assert.IsTrue(pluginContainer.Initialise());
            Assert.AreEqual(3, pluginContainer.Functions.Count);
            Assert.IsTrue(pluginContainer.Functions.Contains(typeof(TestImportableFunctionA) as IFunction));
            Assert.IsTrue(pluginContainer.Functions.Contains(typeof(TestImportableFunctionB) as IFunction));
            Assert.IsTrue(pluginContainer.Functions.Contains(typeof(TestImportableStartFunction) as IFunction));
        }

        [TestMethod]
        public void Valid()
        {
            var pluginContainer = new PluginContainer(null);
            pluginContainer.Initialise();

            Assert.IsFalse(pluginContainer.Valid);
            Assert.IsNull(pluginContainer.Plugin);
            Assert.AreEqual(0, pluginContainer.Functions.Count);

            pluginContainer = new PluginContainer(new TestPlugin()
            {
                PublicName = null,
                PublicAuthor = "An author",
                PublicDescription = "A description"
            });
            pluginContainer.Initialise();

            Assert.IsFalse(pluginContainer.Valid);
            Assert.IsNotNull(pluginContainer.Plugin);
            Assert.AreEqual(0, pluginContainer.Functions.Count);

            pluginContainer = new PluginContainer(new TestPlugin()
            {
                PublicAuthor = "An author",
                PublicDescription = "A description",
                PublicName = "A name"
            });
            pluginContainer.Initialise();

            Assert.IsTrue(pluginContainer.Valid);
            Assert.IsNotNull(pluginContainer.Plugin);
            Assert.AreEqual(3, pluginContainer.Functions.Count);
        }
    }
}
