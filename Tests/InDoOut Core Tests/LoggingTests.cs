using InDoOut_Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Reflection;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class LoggingTests
    {
        [TestMethod]
        public void AddToLog()
        {
            var log = new Log();

            Assert.AreEqual(0, log.Logs.Count);

            log.Error("This is an error");

            Assert.AreEqual(1, log.Logs.Count);
            Assert.AreEqual("This is an error", log.Logs[0].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[0].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Error, log.Logs[0].Level);
            Assert.IsTrue((DateTime.Now - log.Logs[0].Time).TotalSeconds < 1d);

            log.Warning("This is a warning");

            Assert.AreEqual(2, log.Logs.Count);
            Assert.AreEqual("This is a warning", log.Logs[1].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[1].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Warning, log.Logs[1].Level);

            Assert.AreEqual("This is an error", log.Logs[0].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[0].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Error, log.Logs[0].Level);

            log.Info("This is a message");

            Assert.AreEqual(3, log.Logs.Count);
            Assert.AreEqual("This is a message", log.Logs[2].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[2].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Info, log.Logs[2].Level);
            Assert.IsTrue((DateTime.Now - log.Logs[2].Time).TotalSeconds < 1d);

            Assert.AreEqual("This is a warning", log.Logs[1].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[1].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Warning, log.Logs[1].Level);

            Assert.AreEqual("This is an error", log.Logs[0].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[0].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Error, log.Logs[0].Level);

            log.MaxLogMessages = 2;

            log.Info("This is another message");

            Assert.AreEqual(2, log.Logs.Count);
            Assert.AreEqual("This is another message", log.Logs[1].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[1].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Info, log.Logs[1].Level);
            Assert.IsTrue((DateTime.Now - log.Logs[1].Time).TotalSeconds < 1d);

            Assert.AreEqual("This is a message", log.Logs[0].Message);
            Assert.AreEqual(Assembly.GetExecutingAssembly(), log.Logs[0].CallingAssembly);
            Assert.AreEqual(LogMessage.LogLevel.Info, log.Logs[0].Level);
        }
    }
}
