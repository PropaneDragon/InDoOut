using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace InDoOut_Plugins_Tests
{
    [TestClass]
    public class PluginLoaderTests
    {
        [TestMethod]
        public void LoadPluginFromAssembly()
        {
            var validAssembly = Assembly.LoadFrom("InDoOut Desktop API Tests.dll");
            var invalidAssembly = Assembly.LoadFrom("Newtonsoft.Json.dll");
            var pluginLoader = new PluginLoader();

            Assert.IsNotNull(validAssembly);

            var loadedPluginContainer = pluginLoader.LoadPlugin(validAssembly);

            Assert.IsNotNull(loadedPluginContainer);
            Assert.IsNotNull(loadedPluginContainer.Plugin);
            Assert.IsNull(pluginLoader.LoadPlugin((Assembly)null));
            Assert.IsNull(pluginLoader.LoadPlugin(invalidAssembly));
        }

        [TestMethod]
        public void LoadPluginFromPath()
        {
            var pluginLoader = new PluginLoader();
            var loadedPluginContainer = pluginLoader.LoadPlugin("InDoOut Desktop API Tests.dll");

            Assert.IsNotNull(loadedPluginContainer);
            Assert.IsNotNull(loadedPluginContainer.Plugin);
            Assert.IsNull(pluginLoader.LoadPlugin((string)null));
            Assert.IsNull(pluginLoader.LoadPlugin("Newtonsoft.Json.dll"));
            Assert.IsNull(pluginLoader.LoadPlugin("not a valid name"));
        }
    }
}
