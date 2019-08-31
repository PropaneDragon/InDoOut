using InDoOut_Core.Instancing;
using InDoOut_Core.Threading.Safety;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace InDoOut_Philips_Hue_Plugins
{
    internal class JsonFromUrl : Singleton<JsonFromUrl>
    {
        private readonly HttpClient _client = null;

        public enum Method
        {
            GET,
            POST
        }

        public JsonFromUrl()
        {
            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            _client = new HttpClient(clientHandler) { Timeout = TimeSpan.FromSeconds(5) };
        }

        public async Task SendUrl(Uri location, Method method, string content = null)
        {
            _ = await ContentStringFromUrl(location, method, content);
        }

        public async Task<JArray> JsonArrayFromUrl(Uri location, Method method, string content = null)
        {
            var contentString = await ContentStringFromUrl(location, method, content);

            return !string.IsNullOrEmpty(contentString) ? await Task.Run(() => TryGet.ValueOrDefault(() => JArray.Parse(contentString), null)) : null;
        }

        public async Task<JObject> JsonObjectFromUrl(Uri location, Method method, string content = null)
        {
            var contentString = await ContentStringFromUrl(location, method, content);

            return !string.IsNullOrEmpty(contentString) ? await Task.Run(() => TryGet.ValueOrDefault(() => JObject.Parse(contentString), null)) : null;
        }

        public async Task<T> JsonObjectFromUrl<T>(Uri location, Method method, string content = null) where T : class
        {
            var jsonObject = await JsonObjectFromUrl(location, method, content);

            return await Task.Run(() => TryGet.ValueOrDefault(() => jsonObject.ToObject<T>(), null));
        }

        private async Task<string> ContentStringFromUrl(Uri location, Func<Task<HttpResponseMessage>> responseFunction)
        {
            if (!string.IsNullOrEmpty(location.ToString()))
            {
                var responseMessage = await responseFunction.Invoke();
                if (responseMessage.IsSuccessStatusCode)
                {
                    return await TryGet.ValueOrDefault(() => responseMessage.Content.ReadAsStringAsync(), Task.Run(() => (string)null));
                }
            }

            return null;
        }

        private async Task<string> ContentStringFromUrl(Uri location, Method method, string content = null)
        {
            return method switch
            {
                Method.GET => await GetContentStringFromUrl(location),
                Method.POST => await PostContentStringFromUrl(location, content),

                _ => null,
            };
        }

        private async Task<string> GetContentStringFromUrl(Uri location) => await ContentStringFromUrl(location, () => _client.GetAsync(location));
        private async Task<string> PostContentStringFromUrl(Uri location, string content = null) => await ContentStringFromUrl(location, () => _client.PostAsync(location, new StringContent(content)));
    }
}
