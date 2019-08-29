using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;

#if (!_WINDOWS)
using System.Device.Gpio;
#endif

namespace InDoOut_GPIO_Plugins.GPIO
{
    public class PinCountFunction : Function
    {
        public IOutput _outputCounted = null;
        public IResult _resultCount = null;

        public override string Description => "Counts the number of available GPIO pins on the current board.";

        public override string Name => "Count pins";

        public override string Group => "GPIO";

        public override string[] Keywords => new[] { "gpio", "pins", "raspberry", "pi", "IO", "system.device.gpio", "gpiocontroller" };

        public PinCountFunction()
        {
            _ = CreateInput("Count pins");
            _outputCounted = CreateOutput("Counted");
            _resultCount = AddResult(new Result("Number of pins", "The number of pins returned by the board."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
#if (_WINDOWS)
            _ = _resultCount.ValueFrom(0);
#else
            using (var controller = new GpioController())
            {
                _ = _resultCount.ValueFrom(TryGet.ValueOrDefault(() => controller.PinCount, 0));
            }
#endif

            return _outputCounted;
        }
    }
}
