using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Text
{
    public class TextLengthFunction : Function
    {
        private readonly IOutput _output = null;
        private readonly IProperty<string> _text = null;
        private readonly IResult _length = null;

        public override string Description => "Outputs the length of the given text (including spaces).";

        public override string Name => "Count text";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "length", "count", "size", "number", "text", "string", "characters" };

        public TextLengthFunction()
        {
            _ = CreateInput("Count");
            _output = CreateOutput();

            _text = AddProperty(new Property<string>("Text", "The text to count", false, ""));
            _length = AddResult(new Result("Text count", "The number of characters in the text (including spaces).", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var text = _text.FullValue;
            if (text != null)
            {
                _ = _length.ValueFrom(text.Length);
            }

            return _output;
        }
    }
}
