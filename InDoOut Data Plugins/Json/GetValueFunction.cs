using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json.Linq;
using System;

namespace InDoOut_Data_Plugins.Json
{
    public class GetValueFunction : Function
    {
        private readonly IOutput _output, _error;
        private readonly IProperty<string> _json;
        private readonly IResult _value;

        public override string Description => "Gets the value of a JSON object";

        public override string Name => "Get JSON value";

        public override string Group => "JSON";

        public override string[] Keywords => new[] { "" };

        public override IOutput TriggerOnFailure => _error;

        public GetValueFunction()
        {
            _ = CreateInput();

            _output = CreateOutput();
            _error = CreateOutput("Error", OutputType.Negative);

            _json = AddProperty(new Property<string>("JSON input", "The JSON object to get the value for."));

            _value = AddResult(new Result("Value", "The value of the given JSON object"));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            var jsonValue = JToken.Parse(_json.FullValue);
            return jsonValue != null ? (_value.ValueFrom(jsonValue.Value<string>()) ? _output : _error) : _error;
        }
    }
}
