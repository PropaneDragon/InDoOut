using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;

namespace InDoOut_Display_Basic_Plugins.Text
{
    public partial class TextBlockUIElement : DisplayElement
    {
        public TextBlockUIElement(IElementFunction element) : base(element)
        {
            InitializeComponent();
        }

        protected override bool UpdateRequested(IElementFunction element)
        {
            if (element is TextBlockElementFunction textBlockFunction)
            {
                Text_Main.Text = textBlockFunction.Text;

                return true;
            }

            return false;
        }
    }
}
