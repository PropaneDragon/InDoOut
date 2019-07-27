using InDoOut_Desktop.Location;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace InDoOut_Desktop_Tests
{
    [TestClass]
    public class StandardLocationTests
    {
        [TestMethod]
        public void AssemblyLocations()
        {
            var standardLocation = new StandardLocations();
            var startupAssemblyLocation = Assembly.GetEntryAssembly().Location;

            Assert.AreEqual(startupAssemblyLocation, standardLocation.GetPathTo(Location.ApplicationExecutable));
            Assert.AreEqual(Path.GetDirectoryName(startupAssemblyLocation), standardLocation.GetPathTo(Location.ApplicationDirectory));
        }

        [TestMethod]
        public void DefaultValues()
        {
            var standardLocation = new TestStandardLocations();
            var startupAssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var separator = Path.DirectorySeparatorChar;

            Assert.AreEqual($"{separator}Plugins", standardLocation.GetPathTo(Location.PluginsDirectory).Replace(startupAssemblyDirectory, ""));
            Assert.AreEqual($"{separator}Settings.xml", standardLocation.GetPathTo(Location.SettingsFile).Replace(startupAssemblyDirectory, ""));
            Assert.IsNull(standardLocation.GetPathTo(Location.SaveFile));
        }

        [TestMethod]
        public void SetGet()
        {
            var standardLocation = new TestStandardLocations();
            var separator = Path.DirectorySeparatorChar;

            Assert.IsTrue(standardLocation.SetPathTo(Location.PluginsDirectory, @"/A new directory\"));
            Assert.AreEqual($"{separator}A new directory", standardLocation.GetPathTo(Location.PluginsDirectory));

            Assert.IsTrue(standardLocation.SetPathTo(Location.PluginsDirectory, @"\A directory further down/childDirectory/A new directory\"));
            Assert.AreEqual($"{separator}A directory further down{separator}childDirectory{separator}A new directory", standardLocation.GetPathTo(Location.PluginsDirectory));
        }

        [TestMethod]
        public void DirectorySeparator()
        {
            var standardLocation = new TestStandardLocations();
            var separator = Path.DirectorySeparatorChar;

            Assert.IsTrue(standardLocation.SetPathTo(Location.PluginsDirectory, @"\Backslashes\Are\Accepted"));
            Assert.AreEqual($"{separator}Backslashes{separator}Are{separator}Accepted", standardLocation.GetPathTo(Location.PluginsDirectory));

            Assert.IsTrue(standardLocation.SetPathTo(Location.PluginsDirectory, @"/Forward/Slashes/Are/Accepted"));
            Assert.AreEqual($"{separator}Forward{separator}Slashes{separator}Are{separator}Accepted", standardLocation.GetPathTo(Location.PluginsDirectory));

            Assert.IsTrue(standardLocation.SetPathTo(Location.PluginsDirectory, @"\Mixed/Slashes\Should\Be/Accepted/Hopefully"));
            Assert.AreEqual($"{separator}Mixed{separator}Slashes{separator}Should{separator}Be{separator}Accepted{separator}Hopefully", standardLocation.GetPathTo(Location.PluginsDirectory));

            Assert.IsTrue(standardLocation.SetPathTo(Location.PluginsDirectory, @"Forward/Ending/Slash/"));
            Assert.AreEqual($"Forward{separator}Ending{separator}Slash", standardLocation.GetPathTo(Location.PluginsDirectory));

            Assert.IsTrue(standardLocation.SetPathTo(Location.PluginsDirectory, @"Backward\Ending\Slash\"));
            Assert.AreEqual($"Backward{separator}Ending{separator}Slash", standardLocation.GetPathTo(Location.PluginsDirectory));
        }
    }
}
