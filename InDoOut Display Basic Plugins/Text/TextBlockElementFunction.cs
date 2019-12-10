using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using System;

namespace InDoOut_Display_Basic_Plugins.Text
{
    public class TextBlockElementFunction : ElementFunction
    {
        public override string Description => throw new NotImplementedException();

        public override string Name => throw new NotImplementedException();

        public override string Group => throw new NotImplementedException();

        public override string[] Keywords => throw new NotImplementedException();

        public override IElement CreateAssociatedElement()
        {
            return new TextBlockElement();
        }
    }
}
