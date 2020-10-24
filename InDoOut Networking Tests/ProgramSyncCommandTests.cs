using InDoOut_Executable_Core.Networking;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace InDoOut_Executable_Core_Tests
{
    [TestClass]
    public class ProgramSyncCommandTests
    {
        [TestMethod]
        public void CreateStringFromCommand()
        {
            var message = new NetworkMessage("COMMAND", "This is the contents of the command");
            Assert.IsNotNull(message.Id);
            Assert.AreEqual($"{message.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}COMMAND{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}This is the contents of the command", message.ToString());

            message = new NetworkMessage("COMMAND with spaces");
            Assert.IsNotNull(message.Id);
            Assert.AreEqual($"{message.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}COMMAND with spaces{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}", message.ToString());

            message = new NetworkMessage("COMMAND with spaces", null);
            Assert.IsNotNull(message.Id);
            Assert.AreEqual($"{message.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}COMMAND with spaces{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}", message.ToString());

            message = new NetworkMessage("COMMAND with spaces", "This is a section of content", "And this is another", "Does it split?");
            Assert.IsNotNull(message.Id);
            Assert.AreEqual($"{message.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}COMMAND with spaces{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}This is a section of content{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}And this is another{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}Does it split?", message.ToString());

            Assert.IsNull(new NetworkMessage(null, null).ToString());

            Assert.IsTrue(new NetworkMessage("Another command", null).Valid);
            Assert.IsTrue(new NetworkMessage("Another command", "").Valid);
            Assert.IsTrue(new NetworkMessage("Another command", "Data").Valid);

            Assert.IsFalse(new NetworkMessage("", null).Valid);
            Assert.IsFalse(new NetworkMessage("              ", null).Valid);
            Assert.IsFalse(new NetworkMessage("     ", "           ").Valid);
            Assert.IsFalse(new NetworkMessage(null, null).Valid);
        }

        [TestMethod]
        public void CreateFromString()
        {
            var testCommand = NetworkMessage.FromString($"An ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}COMMAND{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}This is the contents of the command");

            Assert.AreEqual(testCommand.Id, "An ID");
            Assert.AreEqual(testCommand.Name, "COMMAND");
            Assert.AreEqual(testCommand.Data.Length, 1);
            Assert.AreEqual(testCommand.Data[0], "This is the contents of the command");

            testCommand = NetworkMessage.FromString($"Another ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}COMMAND{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}This is the contents of the command{NetworkCodes.COMMAND_DATA_GENERIC_SPLITTER}And this is some more!");

            Assert.AreEqual(testCommand.Id, "Another ID");
            Assert.AreEqual(testCommand.Name, "COMMAND");
            Assert.AreEqual(testCommand.Data.Length, 2);
            Assert.AreEqual(testCommand.Data[0], "This is the contents of the command");
            Assert.AreEqual(testCommand.Data[1], "And this is some more!");
            Assert.IsTrue(testCommand.Valid);

            testCommand = NetworkMessage.FromString($"An ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}Another command{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}");

            Assert.AreEqual(testCommand.Id, "An ID");
            Assert.AreEqual(testCommand.Name, "Another command");
            Assert.IsNull(testCommand.Data);
            Assert.IsTrue(testCommand.Valid);

            testCommand = NetworkMessage.FromString($"An ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}Just the data?");
            Assert.IsNull(testCommand);

            testCommand = NetworkMessage.FromString($"An ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}");
            Assert.IsNull(testCommand);

            testCommand = NetworkMessage.FromString($"{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}");
            Assert.IsNull(testCommand);

            testCommand = NetworkMessage.FromString($"This can't evaluate");
            Assert.IsNull(testCommand);

            testCommand = NetworkMessage.FromString($"");
            Assert.IsNull(testCommand);

            testCommand = NetworkMessage.FromString(null);
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
                var testCommand = NetworkMessage.FromString($"ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{text}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}");

                Assert.AreEqual(testCommand.Id, "ID");
                Assert.AreEqual(testCommand.Name, text);
                Assert.IsNull(testCommand.Data);
                Assert.IsTrue(testCommand.Valid);

                testCommand = NetworkMessage.FromString($"ID{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{text}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{text}");

                Assert.AreEqual(testCommand.Id, "ID");
                Assert.AreEqual(testCommand.Name, text);
                Assert.AreEqual(testCommand.Data.Length, 1);
                Assert.AreEqual(testCommand.Data[0], text);
                Assert.IsTrue(testCommand.Valid);

                var testMessage = new NetworkMessage(text);
                Assert.AreEqual($"{testMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{text}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}", testMessage.ToString());

                testMessage = new NetworkMessage(text, text);
                Assert.AreEqual($"{testMessage.Id}{NetworkCodes.MESSAGE_ID_COMMAND_SPLITTER}{text}{NetworkCodes.COMMAND_NAME_DATA_SPLITTER}{text}", testMessage.ToString());
            }
        }
    }
}
