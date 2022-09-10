using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Maths
{
    public class LerpFunction : Function
    {
        private readonly IProperty<double> _minimum, _maximum, _point;

        private readonly IResult _result = new Result("Result value", "The result of the calculation", "0");

        private readonly IOutput _outputCalculated;
        private readonly IOutput _outputFailed;

        public override string Name => "Interpolate";
        public override string Description => "Linear interpolation between two values.";
        public override string Group => "Maths";

        public override string[] Keywords => new[] { "lerp", "interpolate", "linear" };

        public override IOutput TriggerOnFailure => _outputFailed;

        public LerpFunction() : base()
        {
            _minimum = AddProperty(new Property<double>("Start", "The starting point of the linear interpolation", true, 0));
            _maximum = AddProperty(new Property<double>("End", "The ending point of the linear interpolation", true, 100));
            _point = AddProperty(new Property<double>("Interpolation point (%)", "The ending point of the linear interpolation", true, 0));

            _ = AddResult(_result);

            _ = CreateInput("Calculate");

            _outputCalculated = CreateOutput(OutputType.Positive, "Calculated");
            _outputFailed = CreateOutput(OutputType.Negative, "Calculation failed");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            try
            {
                var success = _result.ValueFrom(Calculate());
                if (!success || double.IsNaN(_result.ValueAs<double>()))
                {
                    _ = _result.ValueFrom<double>(0);
                    return _outputFailed;
                }

                return _outputCalculated;
            }
            catch { }

            return _outputFailed;
        }

        protected double Calculate() => Lerp(_minimum.FullValue, _maximum.FullValue, Math.Clamp(_point.FullValue, 0, 100) / 100d);

        private double Lerp(double start, double end, double point) => start * (1 - point) + (end * point);
    }
}
