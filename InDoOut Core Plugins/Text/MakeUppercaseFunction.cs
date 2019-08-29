using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Text
{
    public class MakeUppercaseFunction : Function
    {
        public IOutput _outputDone = null;
        public IProperty<string> _propertyTextIn = null;
        public IResult _resultTextOut = null;

        public override string Description => "Converts text to uppercase.";

        public override string Name => "Make uppercase";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "upper", "case", "convert", "toupper", "loud", "shout", "exclaim", "!", "^", "#" };

        public MakeUppercaseFunction()
        {
            _ = CreateInput("Make uppercase");
            _outputDone = CreateOutput("Done");
            _propertyTextIn = AddProperty(new Property<string>("Text to make uppercase", "The text to convert to uppercase."));
            _resultTextOut = AddResult(new Result("Uppercase text", "The uppercase version of the input text."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (_propertyTextIn.FullValue != null)
            {
                _resultTextOut.RawValue = _propertyTextIn.FullValue.ToUpper();
            }

            return _outputDone;
        }
    }
}
