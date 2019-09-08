using InDoOut_Executable_Core.Location;
using InDoOut_Executable_Core_Tests;
using InDoOut_Plugins.Loaders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace InDoOut_Desktop_Tests
{
    [TestClass]
    public class PluginDirectoryLoaderTests
    {
        [TestMethod]
        public async Task LoadFromStandardLocation()
        {
            var pluginLoader = new PluginLoader();
            var standardLocations = new TestStandardLocations();
            var pluginDirectoryLoader = new PluginDirectoryLoader(pluginLoader, standardLocations);
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            _ = standardLocations.SetPathTo(Location.PluginsDirectory, currentDirectory);

            var pluginsTask = pluginDirectoryLoader.LoadPlugins();
            var waitTask = Task.Delay(TimeSpan.FromSeconds(5));
            var completedTask = await Task.WhenAny(pluginsTask, waitTask);

            Assert.AreEqual(completedTask, pluginsTask);
            Assert.AreEqual(1, pluginsTask.Result.Count);

            var pluginContainer = pluginsTask.Result[0];

            Assert.AreEqual("Default name", pluginContainer.Plugin.Name);
            Assert.IsTrue(pluginContainer.Valid);
            Assert.AreEqual(0, pluginContainer.FunctionTypes.Count);

            Assert.IsTrue(pluginContainer.Initialise());

            Assert.AreEqual(3, pluginContainer.FunctionTypes.Count);
        }

        [TestMethod]
        public async Task LoadFromLocation()
        {
            var pluginLoader = new PluginLoader();
            var pluginDirectoryLoader = new PluginDirectoryLoader(pluginLoader, null);
            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var pluginsTask = pluginDirectoryLoader.LoadPlugins(currentDirectory);
            var waitTask = Task.Delay(TimeSpan.FromSeconds(5));
            var completedTask = await Task.WhenAny(pluginsTask, waitTask);

            Assert.AreEqual(completedTask, pluginsTask);
            Assert.AreEqual(1, pluginsTask.Result.Count);

            var pluginContainer = pluginsTask.Result[0];

            Assert.AreEqual("Default name", pluginContainer.Plugin.Name);
            Assert.IsTrue(pluginContainer.Valid);
            Assert.AreEqual(0, pluginContainer.FunctionTypes.Count);

            Assert.IsTrue(pluginContainer.Initialise());

            Assert.AreEqual(3, pluginContainer.FunctionTypes.Count);
        }

        [TestMethod]
        public async Task Safety()
        {
            var pluginDirectoryLoader = new PluginDirectoryLoader(null, null);

            var pluginsTask = pluginDirectoryLoader.LoadPlugins();
            var waitTask = Task.Delay(TimeSpan.FromSeconds(5));
            var completedTask = await Task.WhenAny(pluginsTask, waitTask);

            Assert.AreEqual(completedTask, pluginsTask);
            Assert.AreEqual(0, pluginsTask.Result.Count);

            pluginsTask = pluginDirectoryLoader.LoadPlugins(null);
            waitTask = Task.Delay(TimeSpan.FromSeconds(5));
            completedTask = await Task.WhenAny(pluginsTask, waitTask);

            Assert.AreEqual(completedTask, pluginsTask);
            Assert.AreEqual(0, pluginsTask.Result.Count);

            pluginsTask = pluginDirectoryLoader.LoadPlugins("A completely invalid path");
            waitTask = Task.Delay(TimeSpan.FromSeconds(5));
            completedTask = await Task.WhenAny(pluginsTask, waitTask);

            Assert.AreEqual(completedTask, pluginsTask);
            Assert.AreEqual(0, pluginsTask.Result.Count);

            var pluginLoader = new PluginLoader();
            var standardLocations = new TestStandardLocations();
            pluginDirectoryLoader = new PluginDirectoryLoader(pluginLoader, standardLocations);

            standardLocations.PublicForcePathTo(Location.PluginsDirectory, null);

            pluginsTask = pluginDirectoryLoader.LoadPlugins();
            waitTask = Task.Delay(TimeSpan.FromSeconds(5));
            completedTask = await Task.WhenAny(pluginsTask, waitTask);

            Assert.AreEqual(completedTask, pluginsTask);
            Assert.AreEqual(0, pluginsTask.Result.Count);

            standardLocations.PublicForcePathTo(Location.PluginsDirectory, "a completely invalid path");

            pluginsTask = pluginDirectoryLoader.LoadPlugins();
            waitTask = Task.Delay(TimeSpan.FromSeconds(5));
            completedTask = await Task.WhenAny(pluginsTask, waitTask);

            Assert.AreEqual(completedTask, pluginsTask);
            Assert.AreEqual(0, pluginsTask.Result.Count);
        }
    }
}
