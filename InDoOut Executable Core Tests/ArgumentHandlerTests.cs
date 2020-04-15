using InDoOut_Executable_Core.Arguments;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Executable_Core_Tests
{
    [TestClass]
    public class ArgumentHandlerTests
    {
        [TestMethod]
        public void ProcessSingular()
        {
            var argumentHandler = new ArgumentHandler();
            var argumentA = new BasicArgument("A", "This is a test", "default", false);

            Assert.IsTrue(argumentHandler.AddArgument(argumentA));
            Assert.AreEqual("default", argumentA.Value);

            var arguments = new[]
            {
                "-A",
                "first"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("first", argumentA.Value);

            arguments = new[]
            {
                "/A",
                "second"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("second", argumentA.Value);

            arguments = new[]
            {
                "-A:third"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("third", argumentA.Value);

            arguments = new[]
            {
                "/A=fourth"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("fourth", argumentA.Value);
        }

        [TestMethod]
        public void ProcessMultiple()
        {
            var argumentHandler = new ArgumentHandler();
            var argumentA = new BasicArgument("A", "This is a test", "default A", false);
            var argumentB = new BasicArgument("B", "This is also test", "default B", false);
            var argumentC = new BasicArgument("C", "This is another test", "default C", false);
            var argumentD = new BasicArgument("D", "This is a final test", "default D", false);

            Assert.IsTrue(argumentHandler.AddArgument(argumentA));
            Assert.IsTrue(argumentHandler.AddArgument(argumentB));
            Assert.IsTrue(argumentHandler.AddArgument(argumentC));
            Assert.IsTrue(argumentHandler.AddArgument(argumentD));
            Assert.AreEqual("default A", argumentA.Value);
            Assert.AreEqual("default B", argumentB.Value);
            Assert.AreEqual("default C", argumentC.Value);
            Assert.AreEqual("default D", argumentD.Value);

            var arguments = new[]
            {
                "-A",
                "first"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("first", argumentA.Value);
            Assert.AreEqual("default B", argumentB.Value);
            Assert.AreEqual("default C", argumentC.Value);
            Assert.AreEqual("default D", argumentD.Value);

            arguments = new[]
            {
                "/B",
                "second",
                "-A:third"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("third", argumentA.Value);
            Assert.AreEqual("second", argumentB.Value);
            Assert.AreEqual("default C", argumentC.Value);
            Assert.AreEqual("default D", argumentD.Value);

            arguments = new[]
            {
                "/B",
                "fourth",
                "-A:fifth",
                "/C=six/ th",
                "-D",
                "seven:th"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("fifth", argumentA.Value);
            Assert.AreEqual("fourth", argumentB.Value);
            Assert.AreEqual("six/ th", argumentC.Value);
            Assert.AreEqual("seven:th", argumentD.Value);
        }

        [TestMethod]
        public void ProcessUnexpected()
        {
            var argumentHandler = new ArgumentHandler();
            var argumentA = new BasicArgument("A", "This is a test", "default A", false);
            var argumentB = new BasicArgument("B", "This is also test", "default B", false);
            var argumentC = new BasicArgument("C", "This is another test", "default C", false);
            var argumentNoValue = new BasicArgument("D", "This is an unsettable test", false, false);

            Assert.IsTrue(argumentHandler.AddArgument(argumentA));
            Assert.IsTrue(argumentHandler.AddArgument(argumentB));
            Assert.IsTrue(argumentHandler.AddArgument(argumentC));
            Assert.IsTrue(argumentHandler.AddArgument(argumentNoValue));
            Assert.AreEqual("default A", argumentA.Value);
            Assert.AreEqual("default B", argumentB.Value);
            Assert.AreEqual("default C", argumentC.Value);

            var arguments = new[]
            {
                "something unexpected",
                "-A",
                "first"
            };

            var exception = Assert.ThrowsException<InvalidArgumentException>(() => argumentHandler.Process(arguments));

            Assert.AreEqual("The value \"something unexpected\" is not an argument, nor is it preceeded by an argument and cannot be processed.", exception.Message);
            Assert.AreEqual("default A", argumentA.Value);
            Assert.AreEqual("default B", argumentB.Value);
            Assert.AreEqual("default C", argumentC.Value);

            arguments = new[]
            {
                "---A",
                "s/econd/",
                "-/-/B",
                ":third:"
            };

            argumentHandler.Process(arguments);

            Assert.AreEqual("s/econd/", argumentA.Value);
            Assert.AreEqual(":third:", argumentB.Value);
            Assert.AreEqual("default C", argumentC.Value);

            arguments = new[]
            {
                "-not an argument",
                "not a value"
            };

            exception = Assert.ThrowsException<InvalidArgumentException>(() => argumentHandler.Process(arguments));

            Assert.AreEqual("The provided argument \"-not an argument\" does not match any known arguments.", exception.Message);
            Assert.AreEqual("s/econd/", argumentA.Value);
            Assert.AreEqual(":third:", argumentB.Value);
            Assert.AreEqual("default C", argumentC.Value);

            arguments = new[]
            {
                "-A",
                "fourth",
                "-B",
                "-C",
                "fifth",
                "-not an argument"
            };

            exception = Assert.ThrowsException<InvalidArgumentException>(() => argumentHandler.Process(arguments));

            Assert.AreEqual("The provided argument \"-not an argument\" does not match any known arguments.", exception.Message);
            Assert.AreEqual("fourth", argumentA.Value);
            Assert.AreEqual(":third:", argumentB.Value);
            Assert.AreEqual("fifth", argumentC.Value);

            arguments = new[]
            {
                "-D"
            };

            argumentHandler.Process(arguments);

            arguments = new[]
            {
                "-D",
                "Can't do this"
            };

            exception = Assert.ThrowsException<InvalidArgumentException>(() => argumentHandler.Process(arguments));

            Assert.AreEqual("The argument \"D\" doesn't allow for values to be set on it.", exception.Message);

            arguments = new[]
            {
                "-D:Can't do this"
            };

            exception = Assert.ThrowsException<InvalidArgumentException>(() => argumentHandler.Process(arguments));

            Assert.AreEqual("The argument \"D\" doesn't allow for values to be set on it.", exception.Message);
        }

        [TestMethod]
        public void Formatting()
        {
            var argumentHandler = new ArgumentHandler();
            var argumentSettable = new BasicArgument("CanBeSet", "This is a description", true);
            var argumentNonSettable = new BasicArgument("CanNotBeSet", "A different description", false);

            Assert.IsTrue(argumentHandler.AddArgument(argumentSettable));
            Assert.IsTrue(argumentHandler.AddArgument(argumentNonSettable));

            Assert.AreEqual("-CanBeSet:[value]", argumentHandler.FormatKeyValue(argumentSettable));
            Assert.AreEqual("-CanNotBeSet", argumentHandler.FormatKeyValue(argumentNonSettable));

            Assert.AreEqual("-CanBeSet:[value] - This is a description", argumentHandler.FormatDescription(argumentSettable));
            Assert.AreEqual("-CanNotBeSet - A different description", argumentHandler.FormatDescription(argumentNonSettable));
        }

        [TestMethod]
        public void Triggering()
        {
            var argumentHandler = new ArgumentHandler();
            var aTriggered = false;
            var bTriggered = false;
            var cTriggered = false;
            var dTriggered = false;
            var argumentA = new BasicArgument("A", "Description", "", false, (handler, value) => aTriggered = true);
            var argumentB = new BasicArgument("B", "Description", "", false, (handler, value) => bTriggered = true);
            var argumentC = new BasicArgument("C", "Description", "", false, (handler, value) => cTriggered = true);
            var argumentD = new BasicArgument("D", "Description", "", false, (handler, value) => dTriggered = true);

            Assert.IsTrue(argumentHandler.AddArgument(argumentA));
            Assert.IsTrue(argumentHandler.AddArgument(argumentB));
            Assert.IsTrue(argumentHandler.AddArgument(argumentC));
            Assert.IsTrue(argumentHandler.AddArgument(argumentD));

            var arguments = new[]
            {
                "-A:value",
                "-B",
                "-C",
                "value"
            };

            Assert.IsFalse(aTriggered);
            Assert.IsFalse(bTriggered);
            Assert.IsFalse(cTriggered);
            Assert.IsFalse(dTriggered);

            argumentHandler.Process(arguments);

            Assert.IsTrue(aTriggered);
            Assert.IsTrue(bTriggered);
            Assert.IsTrue(cTriggered);
            Assert.IsFalse(dTriggered);

            aTriggered = false;
            bTriggered = false;
            cTriggered = false;
            dTriggered = false;

            arguments = new[]
            {
                "-D"
            };

            Assert.IsFalse(aTriggered);
            Assert.IsFalse(bTriggered);
            Assert.IsFalse(cTriggered);
            Assert.IsFalse(dTriggered);

            argumentHandler.Process(arguments);

            Assert.IsFalse(aTriggered);
            Assert.IsFalse(bTriggered);
            Assert.IsFalse(cTriggered);
            Assert.IsTrue(dTriggered);
        }
    }
}
