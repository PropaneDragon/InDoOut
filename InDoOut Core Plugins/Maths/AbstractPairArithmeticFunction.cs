﻿using InDoOut_Core.Entities.Functions;

namespace InDoOut_Core_Plugins.Maths
{
    public abstract class AbstractPairArithmeticFunction : Function
    {
        private Property<double> _firstNumber;
        private Property<double> _secondNumber;

        private Result _result = new Result("Result value", "The result of the calculation", "0");

        private IOutput _outputCalculated;
        private IOutput _outputFailed;

        public override string Group => "Maths";

        protected abstract string Verb { get; }

        public AbstractPairArithmeticFunction() : base()
        {
            _firstNumber = AddProperty(new Property<double>("First number", $"The first number to {Verb}", true, 0));
            _secondNumber = AddProperty(new Property<double>("Second number", $"The second number to {Verb}", true, 0));

            _ = AddResult(_result);

            _ = CreateInput("Calculate");

            _outputCalculated = CreateOutput(OutputType.Positive, "Calculated");
            _outputFailed = CreateOutput(OutputType.Negative, "Calculation failed");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            try
            {
                var success = _result.ValueFrom(Calculate(_firstNumber.FullValue, _secondNumber.FullValue));
                if (!success || double.IsNaN(_result.ValueAs<double>()))
                {
                    _result.ValueFrom<double>(0);
                    return _outputFailed;
                }

                return _outputCalculated;
            }
            catch { }

            return _outputFailed;
        }

        protected abstract double Calculate(double first, double second);
    }
}