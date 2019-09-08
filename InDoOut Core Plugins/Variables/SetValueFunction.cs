using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Variables
{
    public class SetValueFunction : Function
    {
        readonly IOutput _output = null;
        readonly IProperty<string> _valueToSet = null;
        readonly IResult _valueSet = null;

        public override string Description => "Sets a value output from an input value.";

        public override string Name => "Set value";

        public override string Group => "Variables";

        public override string[] Keywords => new[] { "values", "variables", "function", "output", "input", "property", "result", "set", "update" };

        public SetValueFunction()
        {
            _ = CreateInput("Set");
            _output = CreateOutput("Value is set");
            _valueToSet = AddProperty(new Property<string>("Value to set", "The value to set."));
            _valueSet = AddResult(new Result("Value out", "The value that has been set."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            _valueSet.RawValue = _valueToSet.FullValue;

            return _output;
        }
    }
}
