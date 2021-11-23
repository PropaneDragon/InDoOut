using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using InDoOut_Plugins.Helpers;
using System.Drawing;

namespace InDoOut_Display_Basic_Plugins.Shapes
{
    public class EllipseElementFunction : ElementFunction
    {
        private readonly ColourPropertiesHelper _colourPropertiesHelper = new();

        public Color Colour => _colourPropertiesHelper.Colour;

        public override string Description => "An ellipse shape that can be sized and coloured according to needs";

        public override string Name => "Ellipse";

        public override string Group => "Shapes";

        public override string[] Keywords => new[] { "circle", "sphere", "round", "circular", "oval" };

        public EllipseElementFunction()
        {
            _ = AddProperty(_colourPropertiesHelper.HuePercentageProperty);
            _ = AddProperty(_colourPropertiesHelper.BrightnessPercentageProperty);
            _ = AddProperty(_colourPropertiesHelper.SaturationPercentageProperty);
        }

        public override IDisplayElement CreateAssociatedUIElement() => new EllipseUIElement(this);
    }
}
