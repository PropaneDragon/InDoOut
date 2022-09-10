using BlinkStickDotNet;
using InDoOut_Core.Entities.Functions;
using InDoOut_Plugins.Helpers;
using System;
using System.Linq;

namespace InDoOut_BlinkStick_Plugins
{
    public class SetLightColourFunction : Function
    {
        private readonly ColourPropertiesHelper _colourPropertiesHelper = new();
        private readonly IOutput _lightChanged, _error;
        private readonly IProperty<string> _lightId, _leds;

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
            _leds = AddProperty(new Property<string>("LED indexes", "This sets single LEDs on the BlinkStick (if available). Separate multiple LED indexes with commas to set more than one at once. Leave empty to set all together.", false));

            _ = AddProperty(_colourPropertiesHelper.HuePercentageProperty);
            _ = AddProperty(_colourPropertiesHelper.SaturationPercentageProperty);
            _ = AddProperty(_colourPropertiesHelper.BrightnessPercentageProperty);
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            using var stick = BlinkStick.FindBySerial(_lightId.FullValue);
            if (stick != null && stick.OpenDevice())
            {
                stick.SetMode(2);

                if (stick.GetColors(out var colourData))
                {
                    var convertedColour = _colourPropertiesHelper.Colour;
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
            }

            return _error;
        }
    }
}
