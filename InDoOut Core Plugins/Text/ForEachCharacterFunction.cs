using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Text
{
    public class ForEachCharacterFunction : LoopFunction
    {
        private string _cachedString = "";

        private readonly IProperty<string> _text = null;
        private readonly IResult _character = null;

        public override string Description => "Loops through some text and returns each character within it.";

        public override string Name => "For each character";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "char", "character", "foreach", "for", "each", "loop", "string", "split" };

        public ForEachCharacterFunction()
        {
            _text = AddProperty(new Property<string>("Text", "The text to loop through.", false, ""));
            _character = AddResult(new Result("Character", "The current character from the text."));
        }

        protected override void PreprocessItems() => _cachedString = _text.FullValue;

        protected override bool PopulateItemDataForIndex(int index)
        {
            if (_cachedString != null && index < _cachedString.Length)
            {
                _character.RawValue = _cachedString[index].ToString();

                return true;
            }

            return false;
        }

        protected override void AllItemsComplete() => _cachedString = "";
    }
}
