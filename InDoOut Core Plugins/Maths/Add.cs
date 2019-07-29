using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Maths
{
    public class Add : Function
    {
        private Property<int> _firstNumber = new Property<int>("First number", "The first number to add", true, 0);
        private Property<int> _secondNumber = new Property<int>("Second number", "The second number to add", true, 0);

        private Result _additionResult = new Result("Result", "The result of the addition", "0");

        private IInput _inputCalculate;
        private IOutput _outputCalculated;

        public override string Name => "Add";

        public override string Description => "Adds two values together";

        public override string Group => "Maths";

        public override string[] Keywords => new[] { "Addition", "Maths", "Plus" };

        public Add() : base()
        {
            AddProperty(_firstNumber);
            AddProperty(_secondNumber);

            AddResult(_additionResult);

            _inputCalculate = CreateInput("Calculate");

            _outputCalculated = CreateOutput("Calculated");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            _additionResult.ValueFrom(_firstNumber.Value + _secondNumber.Value);

            return _outputCalculated;
        }
    }
}
