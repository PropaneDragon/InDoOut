using InDoOut_Display_Core.Elements;
using System.Windows;

namespace InDoOut_Display_Basic_Plugins.Text
{
    public class TextBlockElement : Element<TextBlockElementFunction>
    {
        public override UIElement CreateAssociatedUIElement()
        {
            return new TextBlockUIElement();
        }
    }
}
