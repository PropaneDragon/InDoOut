using InDoOut_Executable_Core.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace InDoOut_Executable_Core_Tests
{
    [TestClass]
    public class ProgramSyncCommandTests
    {
        private static readonly string EXPECTED_SEPARATOR = "\u0004\u0001\u0004";

        [TestMethod]
        public void CreateStringFromCommand()
        {
            Assert.AreEqual($"COMMAND{EXPECTED_SEPARATOR}This is the contents of the command", new ProgramSyncCommand("COMMAND", "This is the contents of the command").FullCommandString);
            Assert.AreEqual($"COMMAND with spaces{EXPECTED_SEPARATOR}", new ProgramSyncCommand("COMMAND with spaces").FullCommandString);
            Assert.AreEqual($"COMMAND with spaces{EXPECTED_SEPARATOR}", new ProgramSyncCommand("COMMAND with spaces", null).FullCommandString);

            Assert.IsNull(new ProgramSyncCommand(null, null).FullCommandString);

            Assert.IsTrue(new ProgramSyncCommand("Another command", null).Valid);
            Assert.IsTrue(new ProgramSyncCommand("Another command", "").Valid);
            Assert.IsTrue(new ProgramSyncCommand("Another command", "Data").Valid);

            Assert.IsFalse(new ProgramSyncCommand("", null).Valid);
            Assert.IsFalse(new ProgramSyncCommand("              ", null).Valid);
            Assert.IsFalse(new ProgramSyncCommand("     ", "           ").Valid);
            Assert.IsFalse(new ProgramSyncCommand(null, null).Valid);
        }

        [TestMethod]
        public void CreateFromString()
        {
            var testCommand = ProgramSyncCommand.ExtractFromCommandString($"COMMAND{EXPECTED_SEPARATOR}This is the contents of the command");
            
            Assert.AreEqual(testCommand.Command, "COMMAND");
            Assert.AreEqual(testCommand.Data, "This is the contents of the command");
            Assert.IsTrue(testCommand.Valid);

            testCommand = ProgramSyncCommand.ExtractFromCommandString($"Another command{EXPECTED_SEPARATOR}");

            Assert.AreEqual(testCommand.Command, "Another command");
            Assert.AreEqual(testCommand.Data, "");
            Assert.IsTrue(testCommand.Valid);

            testCommand = ProgramSyncCommand.ExtractFromCommandString($"{EXPECTED_SEPARATOR}Just the data?");

            Assert.AreEqual(testCommand.Command, "");
            Assert.AreEqual(testCommand.Data, "Just the data?");
            Assert.IsFalse(testCommand.Valid);

            testCommand = ProgramSyncCommand.ExtractFromCommandString($"{EXPECTED_SEPARATOR}");

            Assert.AreEqual(testCommand.Command, "");
            Assert.AreEqual(testCommand.Data, "");
            Assert.IsFalse(testCommand.Valid);

            testCommand = ProgramSyncCommand.ExtractFromCommandString($"This can't evaluate");
            Assert.IsNull(testCommand);

            testCommand = ProgramSyncCommand.ExtractFromCommandString($"");
            Assert.IsNull(testCommand);

            testCommand = ProgramSyncCommand.ExtractFromCommandString(null);
            Assert.IsNull(testCommand);
        }

        [TestMethod]
        public void UTF8()
        {
            var testTextFiles = Directory.GetFiles(".", "*.txt");
            Assert.AreEqual(5, testTextFiles.Length);

            foreach (var testTextFile in testTextFiles)
            {
                var text = File.ReadAllText(testTextFile, Encoding.UTF8).Replace("\r\n", "\n");
                var testCommand = ProgramSyncCommand.ExtractFromCommandString($"{text}{EXPECTED_SEPARATOR}");

                Assert.AreEqual(testCommand.Command, text);
                Assert.AreEqual(testCommand.Data, "");
                Assert.IsTrue(testCommand.Valid);

                testCommand = ProgramSyncCommand.ExtractFromCommandString($"{text}{EXPECTED_SEPARATOR}{text}");

                Assert.AreEqual(testCommand.Command, text);
                Assert.AreEqual(testCommand.Data, text);
                Assert.IsTrue(testCommand.Valid);

                Assert.AreEqual($"{text}{EXPECTED_SEPARATOR}", new ProgramSyncCommand(text).FullCommandString);
                Assert.AreEqual($"{text}{EXPECTED_SEPARATOR}{text}", new ProgramSyncCommand(text, text).FullCommandString);
            }
        }
    }
}
