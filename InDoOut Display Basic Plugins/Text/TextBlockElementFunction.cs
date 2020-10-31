using InDoOut_Core.Entities.Functions;
using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;

namespace InDoOut_Display_Basic_Plugins.Text
{
    public class TextBlockElementFunction : ElementFunction
    {
        public string Text => _text?.FullValue;

        private readonly IProperty<string> _text;

        public override string Description => "Basic text that can be displayed over other items";

        public override string Name => "Text block";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "textblock", "font", "string", "characters" };

        public TextBlockElementFunction()
        {
            _text = AddProperty(new Property<string>("Text", "The text to show", true, "Enter some text"));
        }

        public override IDisplayElement CreateAssociatedUIElement() => new TextBlockUIElement(this);
    }
}
