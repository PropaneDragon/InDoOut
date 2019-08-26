using InDoOut_Core.Entities.Functions;
using System.Windows;

namespace InDoOut_Debug_Plugins.Output
{
    public class ShowValue : Function
    {
        private readonly Property<string> _value = new Property<string>("Value to show", "The value to display", true, "");

        private readonly IOutput _output;

        public override string Name => "Show value";

        public override string Description => "Displays the value of the given input.";

        public override string Group => "Debug";

        public override string[] Keywords => new[] { "Popup", "Debugging", "Test", "Value", "Display", "Show" };

        public ShowValue() : base()
        {
            _ = AddProperty(_value);

            _ = CreateInput("Show");

            _output = CreateOutput("Closed");
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            _ = MessageBox.Show(_value.FullValue);

            return _output;
        }
    }
}
