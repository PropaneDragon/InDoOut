using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Variables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    [TestClass]
    public class PropertyTests
    {
        [TestMethod]
        public void Constructors()
        {
            var property = new Property<int>("test", "This is a description", false, 42);
            Assert.AreEqual("test", property.Name);
            Assert.AreEqual("This is a description", property.Description);
            Assert.IsFalse(property.Required);
            Assert.AreEqual(42, property.Value);
            Assert.AreEqual("42", property.RawValue);

            property = new Property<int>("test", "This is a description", true, 42);
            Assert.IsTrue(property.Required);

            property = new Property<int>("test 2", "This is a description 2");
            Assert.AreEqual("test 2", property.Name);
            Assert.AreEqual("This is a description 2", property.Description);
            Assert.IsFalse(property.Required);
            Assert.AreEqual(default, property.Value);
            Assert.AreEqual("0", property.RawValue);

            property = new Property<int>("test 4", "This is a description 4", false, 48);
            Assert.AreEqual("test 4", property.Name);
            Assert.AreEqual("This is a description 4", property.Description);
            Assert.IsFalse(property.Required);
            Assert.AreEqual(48, property.Value);
            Assert.AreEqual("48", property.RawValue);

            var propertyDouble = new Property<double>("test", "This is a description", false, 515165.5615d);
            var propertyFloat = new Property<float>("test", "This is a description", false, 11233.45f);
            var propertyString = new Property<string>("test", "This is a description", false, "This is a string saved as the value");
            var propertyBool = new Property<bool>("test", "This is a description", false, true);

            Assert.AreEqual(515165.5615d, propertyDouble.Value);
            Assert.AreEqual("515165.5615", propertyDouble.RawValue);

            Assert.AreEqual(11233.45f, propertyFloat.Value);
            Assert.AreEqual("11233.45", propertyFloat.RawValue);

            Assert.AreEqual("This is a string saved as the value", propertyString.Value);
            Assert.AreEqual("This is a string saved as the value", propertyString.RawValue);

            Assert.IsTrue(propertyBool.Value);
            Assert.AreEqual("True", propertyBool.RawValue);
        }

        [TestMethod]
        public void Value()
        {
            var property = new Property<int>("test", "This is a description", false, 42);
            var variable = new Variable("test", "567");

            Assert.AreEqual(42, property.Value);
            Assert.AreEqual(42, property.ComputedValue);
            Assert.AreEqual("42", property.RawValue);
            Assert.AreEqual("42", property.RawComputedValue);

            property.Value = int.MaxValue;
            Assert.AreEqual(int.MaxValue, property.Value);
            Assert.AreEqual(int.MaxValue, property.ComputedValue);
            Assert.AreEqual(int.MaxValue.ToString(), property.RawValue);
            Assert.AreEqual(int.MaxValue.ToString(), property.RawComputedValue);

            property.RawValue = "-264";
            Assert.AreEqual(-264, property.Value);
            Assert.AreEqual(-264, property.ComputedValue);
            Assert.AreEqual("-264", property.RawValue);
            Assert.AreEqual("-264", property.RawComputedValue);

            property.RawValue = "melon";
            Assert.AreEqual(default, property.Value);
            Assert.AreEqual(default, property.ComputedValue);
            Assert.AreEqual("melon", property.RawValue);
            Assert.AreEqual("melon", property.RawComputedValue);

            property.RawValue = "-51656";
            property.AssociatedVariable = variable;
            Assert.AreEqual(-51656, property.Value);
            Assert.AreEqual(567, property.ComputedValue);
            Assert.AreEqual("-51656", property.RawValue);
            Assert.AreEqual("567", property.RawComputedValue);

            variable.RawValue = "lemon";
            Assert.AreEqual(-51656, property.Value);
            Assert.AreEqual(default, property.ComputedValue);
            Assert.AreEqual("-51656", property.RawValue);
            Assert.AreEqual("lemon", property.RawComputedValue);

            property.AssociatedVariable = null;
            Assert.AreEqual(-51656, property.Value);
            Assert.AreEqual(-51656, property.ComputedValue);
            Assert.AreEqual("-51656", property.RawValue);
            Assert.AreEqual("-51656", property.RawComputedValue);
        }
    }
}
