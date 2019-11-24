using BlinkStickDotNet;
using InDoOut_Core.Entities.Functions;
using System;
using System.Drawing;
using System.Linq;

namespace InDoOut_BlinkStick_Plugins
{
    public class SetLightColourFunction : Function
    {
        private readonly IOutput _lightChanged, _error;
        private readonly IProperty<string> _lightId, _leds;
        private readonly IProperty<double> _huePercentage, _saturationPercentage, _brightnessPercentage;

        public override string Description => "Sets the light colour on a BlinkStick device";

        public override string Name => "Set BlinkStick light colour";

        public override string Group => "BlinkStick";

        public override string[] Keywords => new[] { "lights", "usb" };

        public override IOutput TriggerOnFailure => _error;

        public SetLightColourFunction()
        {
            _ = CreateInput("Change colour");

            _lightChanged = CreateOutput("Light changed", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));
            _huePercentage = AddProperty(new Property<double>("Hue colour (%)", "The hue to set the light to as a percentage, where 100% is red, 33% is green and 66% is blue.", false, 100));
            _saturationPercentage = AddProperty(new Property<double>("Saturation (%)", "The saturation to set the light to as a percentage, where 100% full saturation (colourful) and 0% is no saturation (white).", false, 100));
            _brightnessPercentage = AddProperty(new Property<double>("Brightness (%)", "The brightness to set the light to.", false, 100));
            _leds = AddProperty(new Property<string>("LED indexes", "This sets single LEDs on the BlinkStick (if available). Separate multiple LED indexes with commas to set more than one at once. Leave empty to set all together.", false));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            using var stick = BlinkStick.FindBySerial(_lightId.FullValue);
            if (stick != null && stick.OpenDevice() && stick.GetColors(out var colourData))
            {
                var convertedColour = ColorFromHSV(Math.Clamp(_huePercentage.FullValue, 0d, 100d) * 3.6, Math.Clamp(_saturationPercentage.FullValue, 0d, 100d) / 100d, Math.Clamp(_brightnessPercentage.FullValue, 0d, 100d) / 100d);
                var splitLeds = _leds.FullValue.Split(',');
                var convertedLedIds = splitLeds.Select(ledStringId => int.TryParse(ledStringId, out var ledId) ? ledId : -2);
                var setAll = convertedLedIds.All(ledId => ledId == -2);
                var ledCount = stick.GetLedCount();
                var frameDataCount = 3 * ledCount;

                if (colourData.Length < frameDataCount)
                {
                    colourData = new byte[frameDataCount];

                    for (var index = 0; index < frameDataCount; ++index)
                    {
                        colourData[index] = 0;
                    }
                }

                for (var ledIndex = 0; ledIndex < ledCount; ++ledIndex)
                {
                    var currentFrameOffset = ledIndex * 3;
                    if (convertedLedIds.Contains(ledIndex) || setAll)
                    {
                        colourData[currentFrameOffset] = convertedColour.G;
                        colourData[currentFrameOffset + 1] = convertedColour.R;
                        colourData[currentFrameOffset + 2] = convertedColour.B;
                    }
                }

                stick.SetColors(0, colourData);

                return _lightChanged;
            }

            return _error;
        }

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
