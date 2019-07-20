using InDoOut_Core.Basic;

namespace InDoOut_Core_Tests
{
    internal class TestNamedValue : NamedValue
    {
        public string PublicName { get => Name; set => Name = value; }

        public TestNamedValue(string name, string value = "")
        {
            Name = name;
            RawValue = value;
        }
    }
}
