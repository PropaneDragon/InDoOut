using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Plugins.Maths
{
    public class ClampFunction : AbstractSingularArithmeticFunction
    {
        private readonly IProperty<double> _minimum, _maximum;

        public override string Description => "Clamps a value between two numbers.";

        public override string Name => "Clamp";

        public override string[] Keywords => new[] { "between" };

        protected override string Verb => "clamp";

        public ClampFunction() : base()
        {
            _minimum = AddProperty(new Property<double>("Minimum", "The inclusive minimum allowed number", true, 0));
            _maximum = AddProperty(new Property<double>("Maximum", "The inclusive maximum allowed number", true, 100));
        }

        protected override double Calculate(double number)
        {
            return Math.Clamp(number, _minimum.FullValue, _maximum.FullValue);
        }
    }
}
