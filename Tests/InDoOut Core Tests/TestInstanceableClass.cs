namespace InDoOut_Core_Tests
{
    internal abstract class BaseTestClass
    {
        public bool Instanced { get; protected set; } = false;
    }

    internal class TestDefaultConstructorClass : BaseTestClass
    {
        public TestDefaultConstructorClass()
        {
            Instanced = true;
        }
    }

    internal class TestNoDefaultConstructorClass : BaseTestClass
    {
        private TestNoDefaultConstructorClass()
        {
            Instanced = true;
        }

        public TestNoDefaultConstructorClass(bool _)
        {

        }
    }
}
