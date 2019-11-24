using BlinkStickDotNet;
using InDoOut_Core.Entities.Functions;

namespace InDoOut_BlinkStick_Plugins
{
    public class GetLedCountFunction : Function
    {
        private readonly IOutput _done, _error;
        private readonly IProperty<string> _lightId;
        private readonly IResult _count;

        public override string Description => "Gets the number of LEDs present on a BlinkStick device";

        public override string Name => "Get BlinkStick LED count";

        public override string Group => "BlinkStick";

        public override string[] Keywords => new[] { "lights", "usb" };

        public override IOutput TriggerOnFailure => _error;

        public GetLedCountFunction()
        {
            _ = CreateInput("Get count");

            _done = CreateOutput("Counted", OutputType.Positive);
            _error = CreateOutput("Light error", OutputType.Negative);

            _lightId = AddProperty(new Property<string>("Light ID", "The ID for the light to control.", true));

            _count = AddResult(new Result("Number of LEDs", "The number of LEDs present on the device", "0"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            using var stick = BlinkStick.FindBySerial(_lightId.FullValue);
            return stick != null && stick.OpenDevice() && _count.ValueFrom(stick.GetLedCount()) ? _done : _error;
        }
    }
}
