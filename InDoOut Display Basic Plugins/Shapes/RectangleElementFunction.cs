using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using InDoOut_Plugins.Helpers;
using System.Drawing;

namespace InDoOut_Display_Basic_Plugins.Shapes
{
    public class RectangleElementFunction : ElementFunction
    {
        private readonly ColourPropertiesHelper _colourPropertiesHelper = new ColourPropertiesHelper();

        public Color Colour => _colourPropertiesHelper.Colour;

        public override string Description => "A rectangle shape that can be sized and coloured according to needs";

        public override string Name => "Rectangle";

        public override string Group => "Shapes";

        public override string[] Keywords => new[] { "square", "cube", "box" };

        public RectangleElementFunction()
        {
            _ = AddProperty(_colourPropertiesHelper.HuePercentageProperty);
            _ = AddProperty(_colourPropertiesHelper.BrightnessPercentageProperty);
            _ = AddProperty(_colourPropertiesHelper.SaturationPercentageProperty);
        }

        public override IDisplayElement CreateAssociatedUIElement()
        {
            return new RectangleUIElement(this);
        }
    }
}
