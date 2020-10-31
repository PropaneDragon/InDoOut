using InDoOut_Core.Entities.Functions;
using System;
using System.Drawing;

namespace InDoOut_Plugins.Helpers
{
    /// <summary>
    /// Handles creation of properties relating to setting the colour of an object
    /// in order to provide a consistent interface for entering colour throughout
    /// IDO.
    /// </summary>
    public class ColourPropertiesHelper
    {
        /// <summary>
        /// The title for the <see cref="HuePercentageProperty"/>.
        /// </summary>
        public string HueTitle { get; set; } = "Hue colour (%)";
        /// <summary>
        /// The description for the <see cref="HuePercentageProperty"/>.
        /// </summary>
        public string HueDescription { get; set; } = "The hue to set as a percentage, where 100% is red, 33% is green and 66% is blue.";
        /// <summary>
        /// The title for the <see cref="SaturationPercentageProperty"/>.
        /// </summary>
        public string SaturationTitle { get; set; } = "Saturation (%)";
        /// <summary>
        /// The description for the <see cref="SaturationPercentageProperty"/>.
        /// </summary>
        public string SaturationDescription { get; set; } = "The saturation to set as a percentage, where 100% full saturation (colourful) and 0% is no saturation (white).";
        /// <summary>
        /// The title for the <see cref="BrightnessPercentageProperty"/>.
        /// </summary>
        public string BrightnessTitle { get; set; } = "Brightness (%)";
        /// <summary>
        /// The description for the <see cref="BrightnessPercentageProperty"/>.
        /// </summary>
        public string BrightnessDescription { get; set; } = "The brightness to set as a percentage, where 100% is fully bright and 0% is fully dark.";

        /// <summary>
        /// The property associated with hue percentage.
        /// </summary>
        public IProperty<double> HuePercentageProperty { get; private set; } = null;
        /// <summary>
        /// The property associated with saturation percentage.
        /// </summary>
        public IProperty<double> SaturationPercentageProperty { get; private set; } = null;
        /// <summary>
        /// The property associated with brightness percentage.
        /// </summary>
        public IProperty<double> BrightnessPercentageProperty { get; private set; } = null;

        /// <summary>
        /// The calculated colour from the values set inside the <see cref="HuePercentageProperty"/>
        /// <see cref="SaturationPercentageProperty"/> and <see cref="BrightnessPercentageProperty"/>.
        /// </summary>
        public Color Colour => CalculateColour();

        /// <summary>
        /// Creates a default colour properties class.
        /// </summary>
        public ColourPropertiesHelper()
        {
            CreateColourProperties();
        }

        private void CreateColourProperties()
        {
            HuePercentageProperty = new Property<double>(HueTitle, HueDescription, false, 100);
            SaturationPercentageProperty = new Property<double>(SaturationTitle, SaturationDescription, false, 100);
            BrightnessPercentageProperty = new Property<double>(BrightnessTitle, BrightnessDescription, false, 100);
        }

        private Color CalculateColour() => ColorFromHSV(Math.Clamp(HuePercentageProperty.FullValue, 0d, 100d) * 3.6, Math.Clamp(SaturationPercentageProperty.FullValue, 0d, 100d) / 100d, Math.Clamp(BrightnessPercentageProperty.FullValue, 0d, 100d) / 100d);

        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = (hue / 60) - Math.Floor(hue / 60);

            value *= 255;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - (f * saturation)));
            var t = Convert.ToInt32(value * (1 - ((1 - f) * saturation)));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
