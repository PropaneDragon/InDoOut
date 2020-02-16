using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using System.Windows.Media;

namespace InDoOut_Display_Basic_Plugins.Shapes
{
    public partial class RectangleUIElement : DisplayElement
    {
        public RectangleUIElement(IElementFunction function) : base(function)
        {
            InitializeComponent();
        }

        protected override bool UpdateRequested(IElementFunction function)
        {
            if (function is RectangleElementFunction elementFunction)
            {
                var rawColour = elementFunction.Colour;
                Rectangle_Main.Fill = new SolidColorBrush(Color.FromArgb(rawColour.A, rawColour.R, rawColour.G, rawColour.B));

                return true;
            }

            return false;
        }
    }
}
