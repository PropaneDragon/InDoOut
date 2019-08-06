using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Core_Tests
{
    internal class TestFullFunction : Function
    {
        public override string Description => "A fullly built test function";

        public override string Name => "Full test function";

        public override string Group => "Test";

        public override string[] Keywords => new[] { "Full", "Test", "Function" };

        public IInput Input;

        public IOutput Output;
        public IOutput FailedOutput;

        public Property<int> IntegerProperty = new Property<int>("An integer property", "This is a property that stores an int", false);
        public Property<double> DoubleProperty = new Property<double>("A double property", "This is a property that stores a double", false);
        public Property<float> FloatProperty = new Property<float>("A float property", "This is a property that stores a float", false);
        public Property<string> StringProperty = new Property<string>("A string property", "This is a property that stores a string", false);

        public Result IntegerResult = new Result("Integer output", "An output that gives the input for an int");
        public Result DoubleResult = new Result("Double output", "An output that gives the input for a double");
        public Result FloatResult = new Result("Float output", "An output that gives the input for a float");
        public Result StringResult = new Result("String output", "An output that gives the input for a string");

        public TestFullFunction() : base()
        {
            AddProperty(IntegerProperty);
            AddProperty(DoubleProperty);
            AddProperty(FloatProperty);
            AddProperty(StringProperty);

            AddResult(IntegerResult);
            AddResult(DoubleResult);
            AddResult(FloatResult);
            AddResult(StringResult);

            Input = CreateInput();

            Output = CreateOutput();
            FailedOutput = CreateOutput(OutputType.Negative, "Failed");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            if (IntegerProperty.Valid && DoubleProperty.Valid && FloatProperty.Valid && StringProperty.Valid)
            {
                IntegerResult.ValueFrom(IntegerProperty.BasicValue);
                DoubleResult.ValueFrom(DoubleProperty.BasicValue);
                FloatResult.ValueFrom(FloatProperty.BasicValue);
                StringResult.ValueFrom(StringProperty.BasicValue);

                return Output;
            }

            return FailedOutput;
        }
    }
}
