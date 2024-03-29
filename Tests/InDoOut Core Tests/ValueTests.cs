﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace InDoOut_Core_Tests
{
    [TestClass]
    public class NamedValueTests
    {
        [TestMethod]
        public void Set()
        {
            var namedValue = new TestValue();

            Assert.AreEqual("", namedValue.RawValue);
            Assert.IsTrue(namedValue.ValidValue);

            namedValue.RawValue = null;

            Assert.IsNull(namedValue.RawValue);
            Assert.IsFalse(namedValue.ValidValue);

            namedValue.RawValue = "Something else";

            Assert.AreEqual("Something else", namedValue.RawValue);
            Assert.IsTrue(namedValue.ValidValue);

            Assert.IsTrue(namedValue.ValueFrom<string>(null));
            Assert.IsNull(namedValue.RawValue);

            Assert.IsTrue(namedValue.ValueFrom(10));
            Assert.AreEqual("10", namedValue.RawValue);

            Assert.IsTrue(namedValue.ValueFrom(-465));
            Assert.AreEqual("-465", namedValue.RawValue);

            Assert.IsTrue(namedValue.ValueFrom(-635165.5641d));
            Assert.AreEqual("-635165.5641", namedValue.RawValue);

            Assert.IsTrue(namedValue.ValueFrom(5646.51f));
            Assert.AreEqual("5646.51", namedValue.RawValue);

            Assert.IsTrue(namedValue.ValueFrom(true));
            Assert.AreEqual("True", namedValue.RawValue);

            Assert.IsTrue(namedValue.ValueFrom(false));
            Assert.AreEqual("False", namedValue.RawValue);

            Assert.IsTrue(namedValue.ValueFrom(new int[] { 10, 52, 24 }));
            Assert.AreNotEqual("False", namedValue.RawValue);
        }

        [TestMethod]
        public void Get()
        {
            var namedValue = new TestValue(null);

            Assert.IsNull(namedValue.RawValue);
            Assert.AreEqual("", namedValue.ValueOrDefault());
            Assert.AreEqual("not null", namedValue.ValueOrDefault("not null"));

            namedValue.RawValue = "Actually not null";

            Assert.AreEqual("Actually not null", namedValue.RawValue);
            Assert.AreEqual("Actually not null", namedValue.ValueOrDefault());
            Assert.AreEqual("Actually not null", namedValue.ValueOrDefault("not null"));

            Assert.AreEqual(default, namedValue.ValueAs<int>());
            Assert.AreEqual(-80085, namedValue.ValueAs(-80085));

            Assert.AreEqual(default, namedValue.ValueAs<double>());
            Assert.AreEqual(123.4546d, namedValue.ValueAs(123.4546));

            Assert.AreEqual(default, namedValue.ValueAs<float>());
            Assert.AreEqual(-65654.51f, namedValue.ValueAs(-65654.51f));

            Assert.AreEqual(default, namedValue.ValueAs<bool>());
            Assert.AreEqual(true, namedValue.ValueAs(true));

            namedValue.RawValue = "-1234";

            Assert.AreEqual(-1234, namedValue.ValueAs<int>());
            Assert.AreEqual(-1234, namedValue.ValueAs(-80085));

            Assert.AreEqual(-1234d, namedValue.ValueAs<double>());
            Assert.AreEqual(-1234d, namedValue.ValueAs(123.4546));

            Assert.AreEqual(-1234f, namedValue.ValueAs<float>());
            Assert.AreEqual(-1234f, namedValue.ValueAs(-65654.5156f));

            Assert.AreEqual(default, namedValue.ValueAs<bool>());
            Assert.AreEqual(true, namedValue.ValueAs(true));

            namedValue.RawValue = "5678.92";

            Assert.AreEqual(default, namedValue.ValueAs<int>());
            Assert.AreEqual(-80085, namedValue.ValueAs(-80085));

            Assert.AreEqual(5678.92d, namedValue.ValueAs<double>());
            Assert.AreEqual(5678.92d, namedValue.ValueAs(123.4546));

            Assert.AreEqual(5678.92f, namedValue.ValueAs<float>());
            Assert.AreEqual(5678.92f, namedValue.ValueAs(-65654.5156f));

            Assert.AreEqual(default, namedValue.ValueAs<bool>());
            Assert.AreEqual(true, namedValue.ValueAs(true));

            namedValue.RawValue = "true";

            Assert.AreEqual(default, namedValue.ValueAs<int>());
            Assert.AreEqual(-80085, namedValue.ValueAs(-80085));

            Assert.AreEqual(default, namedValue.ValueAs<double>());
            Assert.AreEqual(123.4546d, namedValue.ValueAs(123.4546));

            Assert.AreEqual(default, namedValue.ValueAs<float>());
            Assert.AreEqual(-65654.51f, namedValue.ValueAs(-65654.51f));

            Assert.AreEqual(true, namedValue.ValueAs<bool>());
            Assert.AreEqual(true, namedValue.ValueAs(true));

            namedValue.RawValue = "1";

            Assert.AreEqual(1, namedValue.ValueAs<int>());
            Assert.AreEqual(1, namedValue.ValueAs(-80085));

            Assert.AreEqual(1d, namedValue.ValueAs<double>());
            Assert.AreEqual(1d, namedValue.ValueAs(123.4546));

            Assert.AreEqual(1f, namedValue.ValueAs<float>());
            Assert.AreEqual(1f, namedValue.ValueAs(-65654.51f));

            Assert.AreEqual(default, namedValue.ValueAs<bool>());
            Assert.AreEqual(true, namedValue.ValueAs(true));
        }

        [TestMethod]
        public void Valid()
        {
            var variable = new TestValue("test");

            Assert.IsTrue(variable.ValidValue);

            variable.RawValue = "";
            Assert.IsTrue(variable.ValidValue);

            variable.RawValue = null;
            Assert.IsFalse(variable.ValidValue);

            variable.RawValue = "Something";
            Assert.IsTrue(variable.ValidValue);

            variable.RawValue = null;
            Assert.IsFalse(variable.ValidValue);

            variable.RawValue = "Something";
            Assert.IsTrue(variable.ValidValue);
        }
    }
}
