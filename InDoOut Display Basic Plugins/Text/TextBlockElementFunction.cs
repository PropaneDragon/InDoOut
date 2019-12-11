using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;

namespace InDoOut_Display_Basic_Plugins.Text
{
    public class TextBlockElementFunction : ElementFunction
    {
        public override string Description => "Basic text that can be displayed over other items";

        public override string Name => "Text block";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "textblock", "font", "string", "characters" };

        public override IElement CreateAssociatedElement()
        {
            return new TextBlockElement();
        }
    }
}
