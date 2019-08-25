using InDoOut_Core.Entities.Functions;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Core_Plugins.Text
{
    public class CombineTextFunction : Function
    {
        private static readonly int NUMBER_PROPERTIES_TO_CRETATE = 10;

        private IOutput _output = null;
        private IResult _result = null;
        private List<IProperty> _properties = new List<IProperty>();

        public override string Description => "Combines two or more text elements together.";

        public override string Name => "Combine text";

        public override string Group => "Text";

        public override string[] Keywords => new[] { "Concatenate", "Group", "Text", "String", "Merge" };

        public CombineTextFunction()
        {
            _ = CreateInput("Combine");

            _output = CreateOutput();
            _result = AddResult(new Result("Combined text", "The combined text from the inputs", ""));

            for (var count = 1; count <= NUMBER_PROPERTIES_TO_CRETATE; ++count)
            {
                _properties.Add(AddProperty(new Property<string>($"String {count}", $"String number {count} to combine.", false, "")));
            }
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            _result.RawValue = string.Concat(_properties.Select(property => property.RawValue));
            return _output;
        }
    }
}
