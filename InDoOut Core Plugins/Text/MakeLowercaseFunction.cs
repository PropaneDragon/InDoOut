using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Text
{
    public class MakeLowercaseFunction : Function
    {
        public IOutput _outputDone = null;
        public IProperty<string> _propertyTextIn = null;
        public IResult _resultTextOut = null;

        public override string Description => "Converts text to lowercase.";

        public override string Name => "Make lowercase";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "lower", "case", "convert", "tolower", "quiet", "small" };

        public MakeLowercaseFunction()
        {
            _ = CreateInput("Make lowercase");
            _outputDone = CreateOutput("Done");
            _propertyTextIn = AddProperty(new Property<string>("Text to make lowercase", "The text to convert to lowercase."));
            _resultTextOut = AddResult(new Result("Lowercase text", "The lowercase version of the input text."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (_propertyTextIn.FullValue != null)
            {
                _resultTextOut.RawValue = _propertyTextIn.FullValue.ToLower();
            }

            return _outputDone;
        }
    }
}
