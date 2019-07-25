using InDoOut_Core.Instancing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InDoOut_Core_Tests
{
    class TopLevelClass { }
    class InheritedClass : TopLevelClass { }
    class CompletelyDifferentClass { }

    [TestClass]
    public class InstanceBuilderTests
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            var instanceBuilder = new InstanceBuilder<object>();
            var goodInstance = instanceBuilder.BuildInstance(typeof(TestDefaultConstructorClass));

            Assert.IsNotNull(goodInstance);
            Assert.AreEqual(typeof(TestDefaultConstructorClass), goodInstance.GetType());
            Assert.IsTrue((goodInstance as TestDefaultConstructorClass).Instanced);

            goodInstance = instanceBuilder.BuildInstance<TestDefaultConstructorClass>();

            Assert.IsNotNull(goodInstance);
            Assert.AreEqual(typeof(TestDefaultConstructorClass), goodInstance.GetType());
            Assert.IsTrue((goodInstance as TestDefaultConstructorClass).Instanced);

            Assert.IsNull(instanceBuilder.BuildInstance(typeof(TestNoDefaultConstructorClass)));
            Assert.IsNull(instanceBuilder.BuildInstance<TestNoDefaultConstructorClass>());
        }

        [TestMethod]
        public void Types()
        {
            var instanceBuilder = new InstanceBuilder<TopLevelClass>();
            var instance = instanceBuilder.BuildInstance(typeof(TopLevelClass));

            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(TopLevelClass), instance.GetType());

            instance = instanceBuilder.BuildInstance<TopLevelClass>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(TopLevelClass), instance.GetType());

            instance = instanceBuilder.BuildInstance(typeof(InheritedClass));

            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(InheritedClass), instance.GetType());

            instance = instanceBuilder.BuildInstance<InheritedClass>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(InheritedClass), instance.GetType());

            instance = instanceBuilder.BuildInstance(typeof(CompletelyDifferentClass));

            Assert.IsNull(instance);

            var instanceBuilder2 = new InstanceBuilder<InheritedClass>();
            instance = instanceBuilder2.BuildInstance(typeof(InheritedClass));

            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(InheritedClass), instance.GetType());

            instance = instanceBuilder.BuildInstance<InheritedClass>();

            Assert.IsNotNull(instance);
            Assert.AreEqual(typeof(InheritedClass), instance.GetType());

            instance = instanceBuilder2.BuildInstance(typeof(TopLevelClass));

            Assert.IsNull(instance);

            instance = instanceBuilder2.BuildInstance(typeof(CompletelyDifferentClass));

            Assert.IsNull(instance);
        }
    }
}
