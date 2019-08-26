using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Numeric
{
    public class IsANumberFunction : Function
    {
        private IOutput _isNumber, _notNumber;
        private IProperty<string> _value;

        public override string Description => "Checks whether a value is a number or not.";

        public override string Name => "Is a number";

        public override string Group => "Numeric";

        public override string[] Keywords => new[] { "valid", "validity", "sanity", "number" };

        public IsANumberFunction()
        {
            _ = CreateInput("Check");

            _isNumber = CreateOutput("Is a number", OutputType.Positive);
            _notNumber = CreateOutput("Not a number", OutputType.Negative);

            _value = AddProperty(new Property<string>("Value", "The value to check"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            return !string.IsNullOrEmpty(_value.FullValue) && double.TryParse(_value.FullValue, out var _) ? _isNumber : _notNumber;
        }
    }
}
