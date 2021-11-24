using InDoOut_Core.Entities.Functions;
using System.Net.Http;

namespace InDoOut_Data_Plugins.Web
{
    public class DownloadTextFunction : Function
    {
        private readonly IOutput _downloaded, _error;
        private readonly IProperty<string> _url;
        private readonly IResult _text;

        public override string Description => "Downloads text from a given URL";

        public override string Name => "Download text";

        public override string Group => "Web";

        public override string[] Keywords => new[] { "download", "txt", "contents", "page", "webpage", "url", "api", "json", "xml", "html", "css" };

        public override IOutput TriggerOnFailure => _error;

        public DownloadTextFunction()
        {
            _ = CreateInput("Download");

            _downloaded = CreateOutput("Downloaded", OutputType.Positive);
            _error = CreateOutput("Error", OutputType.Negative);

            _url = AddProperty(new Property<string>("URL", "The URL to download the text from.", true));

            _text = AddResult(new Result("Downloaded text", "The download text from the given URL."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            using var client = new HttpClient();
            return _text.ValueFrom(client.GetAsync(_url.FullValue).Result.Content.ReadAsStringAsync().Result) ? _downloaded : _error;
        }
    }
}
