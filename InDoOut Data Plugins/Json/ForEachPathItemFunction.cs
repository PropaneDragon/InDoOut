using InDoOut_Core.Entities.Functions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace InDoOut_Data_Plugins.Json
{
    public class ForEachPathItemFunction : LoopFunction
    {
        private readonly IProperty<string> _json, _pathQuery;
        private readonly IResult _jsonResult;

        private IEnumerable<JToken> _tokens = null;

        public override string Description => "Loops through all returned results from a JSON Path query.";

        public override string Name => "For each JSON path item";

        public override string Group => "JSON";

        public override string[] Keywords => new[] { "$", "@", "~", "." };

        public ForEachPathItemFunction() : base()
        {
            _json = AddProperty(new Property<string>("JSON to query", "The full JSON data to query with the path query.", true));
            _pathQuery = AddProperty(new Property<string>("Path query", "A full query path to run on the incoming JSON values", true));

            _jsonResult = AddResult(new Result("JSON result", "The result of the query on the incoming JSON."));
        }

        protected override void AllItemsComplete()
        {
            _tokens = null;
        }

        protected override bool PopulateItemDataForIndex(int index)
        {
            if (_tokens != null && index < _tokens.Count())
            {
                var element = _tokens.ElementAt(index);
                if (element != null)
                {
                    return _jsonResult.ValueFrom(element.ToString(Formatting.Indented));
                }
            }

            return false;
        }

        protected override void PreprocessItems()
        {
            if (!string.IsNullOrEmpty(_json.FullValue) && !string.IsNullOrEmpty(_pathQuery.FullValue))
            {
                try
                {
                    var parsedJson = JObject.Parse(_json.FullValue);
                    if (parsedJson != null)
                    {
                        _tokens = parsedJson.SelectTokens(_pathQuery.FullValue);
                    }
                }
                catch { }
            }
        }
    }
}
