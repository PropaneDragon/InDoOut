using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Text
{
    public class SplitTextFunction : Function
    {
        private readonly IOutput _output = null;
        private readonly IResult _splitLeft = null;
        private readonly IResult _splitRight = null;
        private readonly IProperty<string> _text = null;
        private readonly IProperty<int> _splitPosition = null;

        public override string Description => "Splits text into two from the given position.";

        public override string Name => "Split text";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "split", "extract", "cut", "pair" };

        public SplitTextFunction()
        {
            _ = CreateInput("Split");

            _output = CreateOutput("Done");
            _splitLeft = AddResult(new Result("Text left of split", "The text before the split point.", ""));
            _splitRight = AddResult(new Result("Text right of split", "The text after the split point.", ""));
            _text = AddProperty(new Property<string>("Text to split", "The text to split into two.", true, ""));
            _splitPosition = AddProperty(new Property<int>("Split point", "The point in which to split the text into two."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var textToSplit = _text.FullValue;

            if (_splitPosition.FullValue <= textToSplit.Length && _splitPosition.FullValue > 0)
            {
                _ = _splitLeft.ValueFrom(textToSplit.Substring(0, _splitPosition.FullValue));
                _ = _splitRight.ValueFrom(textToSplit.Substring(_splitPosition.FullValue));
            }
            else
            {
                _ = _splitRight.ValueFrom(_splitPosition.FullValue <= 0 ? _text.FullValue : "");
                _ = _splitLeft.ValueFrom(_splitPosition.FullValue <= 0 ? "" : _text.FullValue);
            }

            return _output;
        }
    }
}
