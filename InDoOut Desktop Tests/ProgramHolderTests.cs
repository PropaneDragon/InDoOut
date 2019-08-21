using InDoOut_Core.Entities.Programs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Desktop_Tests
{
    [TestClass]
    public class ProgramHolderTests
    {
        [TestMethod]
        public void AddProgram()
        {
            var programHolder = new TestProgramHolder();
            var testProgram = new Program();

            Assert.AreEqual(0, programHolder.ProgramsPublic.Count);

            Assert.IsTrue(programHolder.AddProgram(testProgram));
            Assert.AreEqual(1, programHolder.ProgramsPublic.Count);

            Assert.IsTrue(programHolder.AddProgram(new Program()));
            Assert.AreEqual(2, programHolder.ProgramsPublic.Count);

            Assert.IsFalse(programHolder.AddProgram(testProgram));
            Assert.AreEqual(2, programHolder.ProgramsPublic.Count);

            Assert.IsFalse(programHolder.AddProgram(null));
            Assert.AreEqual(2, programHolder.ProgramsPublic.Count);

            Assert.IsFalse(programHolder.AddProgram(programHolder.NewProgram()));
            Assert.AreEqual(3, programHolder.ProgramsPublic.Count);
        }

        [TestMethod]
        public void RemoveProgram()
        {
            var programHolder = new TestProgramHolder();
            var testProgramA = new Program();
            var testProgramB = new Program();

            Assert.AreEqual(0, programHolder.ProgramsPublic.Count);

            Assert.IsTrue(programHolder.AddProgram(testProgramA));
            Assert.AreEqual(1, programHolder.ProgramsPublic.Count);

            Assert.IsFalse(programHolder.RemoveProgram(null));
            Assert.IsFalse(programHolder.RemoveProgram(testProgramB));
            Assert.AreEqual(1, programHolder.ProgramsPublic.Count);

            Assert.IsTrue(programHolder.RemoveProgram(testProgramA));
            Assert.AreEqual(0, programHolder.ProgramsPublic.Count);
            Assert.IsFalse(programHolder.RemoveProgram(testProgramA));

            var newProgram = programHolder.NewProgram();

            Assert.AreEqual(1, programHolder.ProgramsPublic.Count);
            Assert.IsTrue(programHolder.RemoveProgram(newProgram));
            Assert.AreEqual(0, programHolder.ProgramsPublic.Count);
            Assert.IsFalse(programHolder.RemoveProgram(newProgram));
        }

        [TestMethod]
        public void ProgramExists()
        {
            var programHolder = new TestProgramHolder();
            var testProgramA = new Program();
            var testProgramB = new Program();

            Assert.IsFalse(programHolder.ProgramExists(testProgramA));
            Assert.IsFalse(programHolder.ProgramExists(testProgramB));
            Assert.IsFalse(programHolder.ProgramExists(null));

            Assert.IsTrue(programHolder.AddProgram(testProgramA));
            Assert.IsTrue(programHolder.ProgramExists(testProgramA));
            Assert.IsFalse(programHolder.ProgramExists(testProgramB));

            Assert.IsTrue(programHolder.AddProgram(testProgramB));
            Assert.IsTrue(programHolder.ProgramExists(testProgramA));
            Assert.IsTrue(programHolder.ProgramExists(testProgramB));

            Assert.IsTrue(programHolder.RemoveProgram(testProgramA));
            Assert.IsFalse(programHolder.ProgramExists(testProgramA));
            Assert.IsTrue(programHolder.ProgramExists(testProgramB));

            Assert.IsTrue(programHolder.RemoveProgram(testProgramB));
            Assert.IsFalse(programHolder.ProgramExists(testProgramA));
            Assert.IsFalse(programHolder.ProgramExists(testProgramB));

            var newProgram = programHolder.NewProgram();
            Assert.IsNotNull(newProgram);
            Assert.IsFalse(programHolder.ProgramExists(testProgramA));
            Assert.IsFalse(programHolder.ProgramExists(testProgramB));
            Assert.IsTrue(programHolder.ProgramExists(newProgram));

            Assert.IsTrue(programHolder.RemoveProgram(newProgram));
            Assert.IsFalse(programHolder.ProgramExists(testProgramA));
            Assert.IsFalse(programHolder.ProgramExists(testProgramB));
            Assert.IsFalse(programHolder.ProgramExists(newProgram));
        }

        [TestMethod]
        public void NewProgram()
        {
            var programHolder = new TestProgramHolder();

            Assert.AreEqual(0, programHolder.ProgramsPublic.Count);

            var newProgram = programHolder.NewProgram();
            Assert.IsNotNull(newProgram);
            Assert.AreEqual(1, programHolder.ProgramsPublic.Count);

            var otherNewProgram = programHolder.NewProgram();
            Assert.IsNotNull(otherNewProgram);
            Assert.AreEqual(2, programHolder.ProgramsPublic.Count);

            Assert.AreNotEqual(newProgram, otherNewProgram);
        }
    }
}
