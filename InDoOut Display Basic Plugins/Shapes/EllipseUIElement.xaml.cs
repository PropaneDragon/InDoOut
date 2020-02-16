using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using System.Windows.Media;

namespace InDoOut_Display_Basic_Plugins.Shapes
{
    public partial class EllipseUIElement : DisplayElement
    {
        public EllipseUIElement(IElementFunction function) : base(function)
        {
            InitializeComponent();
        }

        protected override bool UpdateRequested(IElementFunction function)
        {
            if (function is EllipseElementFunction elementFunction)
            {
                var rawColour = elementFunction.Colour;
                Ellipse_Main.Fill = new SolidColorBrush(Color.FromArgb(rawColour.A, rawColour.R, rawColour.G, rawColour.B));

                return true;
            }

            return false;
        }
    }
}
