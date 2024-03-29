﻿using InDoOut_Core.Threading.Safety;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class TryGetTests
    {
        [TestMethod]
        public void ValueOrDefault()
        {
            Assert.IsNull(TryGet.ValueOrDefault(() => null, "not null"));
            Assert.IsNull(TryGet.ValueOrDefault<string>(() => throw new Exception("This shouldn't be thrown")));

            Assert.AreEqual(TryGet.ValueOrDefault<int>(() => throw new Exception("This shouldn't be thrown")), default);
            Assert.AreEqual(TryGet.ValueOrDefault<int>(() => throw new Exception("This shouldn't be thrown")), 0);

            Assert.AreEqual(TryGet.ValueOrDefault<bool>(() => throw new Exception("This shouldn't be thrown")), default);
            Assert.AreEqual(TryGet.ValueOrDefault<bool>(() => throw new Exception("This shouldn't be thrown")), false);

            Assert.AreEqual(TryGet.ValueOrDefault<object>(() => throw new Exception("This shouldn't be thrown")), default);
            Assert.IsNull(TryGet.ValueOrDefault<object>(() => throw new Exception("This shouldn't be thrown")));

            Assert.IsNotNull(TryGet.ValueOrDefault(() => throw new Exception("This shouldn't be thrown"), "not null"));
            Assert.AreEqual("not null", TryGet.ValueOrDefault(() => throw new Exception("This shouldn't be thrown"), "not null"));

            Assert.AreNotEqual("not null", TryGet.ValueOrDefault(() => "actually not null", "not null"));
            Assert.AreEqual("actually not null", TryGet.ValueOrDefault(() => "actually not null", "not null"));
        }

        [TestMethod]
        public void ExecuteOrFail()
        {
            var a = 0;

            Assert.IsTrue(TryGet.ExecuteOrFail(() => a = 1));
            Assert.AreEqual(1, a);
            Assert.IsTrue(TryGet.ExecuteOrFail(() => a = -5611));
            Assert.AreEqual(-5611, a);
            Assert.IsFalse(TryGet.ExecuteOrFail(() => a = int.Parse("*")));
            Assert.AreEqual(-5611, a);

            Assert.IsFalse(TryGet.ExecuteOrFail(() => throw new Exception()));
        }
    }
}
