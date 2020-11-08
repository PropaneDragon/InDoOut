using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Q42.HueApi;
using Q42.HueApi.Models;
using Q42.HueApi.Models.Groups;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace InDoOut_Philips_Hue_Plugins
{
    internal static class HueHelpers
    {
        private static readonly CacheControlHeaderValue _cacheControl = new CacheControlHeaderValue { NoCache = true, NoStore = true };
        private static readonly Dictionary<string, HueClient> _clientStorage = new Dictionary<string, HueClient>();

        public static HueClient GetClient(IApiFunction apiFunction) => apiFunction != null ? GetClient(apiFunction.BridgeIPProperty.FullValue, apiFunction.UserIdProperty.FullValue) : null;
        public static HueClient GetClient(string bridgeIp, string userId) => !string.IsNullOrEmpty(bridgeIp) && !string.IsNullOrEmpty(userId) ? TryGet.ValueOrDefault(() => GenerateHueClient(bridgeIp, userId), null) : null;

        public static Group GetGroup(HueClient client, IProperty<string> id) => GetGroup(client, id.FullValue);
        public static Group GetGroup(HueClient client, string id) => client != null && !string.IsNullOrEmpty(id) ? TryGet.ValueOrDefault(() => client.GetGroupAsync(id).Result, null) : null;

        public static Scene GetScene(HueClient client, IProperty<string> id) => GetScene(client, id.FullValue);
        public static Scene GetScene(HueClient client, string id) => client != null && !string.IsNullOrEmpty(id) ? TryGet.ValueOrDefault(() => client.GetSceneAsync(id).Result, null) : null;

        public static Light GetLight(HueClient client, IProperty<string> id) => GetLight(client, id.FullValue);
        public static Light GetLight(HueClient client, string id) => client != null ? TryGet.ValueOrDefault(() => client.GetLightAsync(id).Result, null) : null;

        public static Sensor GetSensor(HueClient client, IProperty<string> id) => GetSensor(client, id.FullValue);
        public static Sensor GetSensor(HueClient client, string id) => client != null ? TryGet.ValueOrDefault(() => client.GetSensorAsync(id).Result, null) : null;

        public static List<Group> GetGroups(HueClient client, IProperty<string> name, bool allowPartialMatches) => GetGroups(client, name.FullValue, allowPartialMatches);
        public static List<Group> GetGroups(HueClient client, string name, bool allowPartialMatches)
        {
            if (client != null)
            {
                var groups = TryGet.ValueOrDefault(() => client.GetGroupsAsync().Result, null);
                if (groups != null)
                {
                    return groups.Where(group => StringMatches(group.Name, name, allowPartialMatches)).ToList();
                }
            }

            return new List<Group>();
        }

        public static List<Scene> GetScenes(HueClient client, Group group, IProperty<string> name, bool allowPartialMatches) => GetScenes(client, group, name.FullValue, allowPartialMatches);
        public static List<Scene> GetScenes(HueClient client, Group group, string name, bool allowPartialMatches) => GetScenes(client, name, allowPartialMatches).Where(scene => scene.Group == group?.Id).ToList();
        public static List<Scene> GetScenes(HueClient client, IProperty<string> name = null, bool allowPartialMatches = false) => GetScenes(client, name.FullValue, allowPartialMatches);
        public static List<Scene> GetScenes(HueClient client, string name, bool allowPartialMatches)
        {
            if (client != null)
            {
                var scenes = TryGet.ValueOrDefault(() => client.GetScenesAsync().Result, null);
                if (scenes != null)
                {
                    return string.IsNullOrEmpty(name) ? scenes.ToList() : scenes.Where(scene => StringMatches(scene.Name, name, allowPartialMatches)).ToList();
                }
            }

            return new List<Scene>();
        }

        public static List<Light> GetLights(HueClient client, Group group, IProperty<string> name, bool allowPartialMatches = false) => GetLights(client, group, name.FullValue, allowPartialMatches);
        public static List<Light> GetLights(HueClient client, Group group, string name = null, bool allowPartialMatches = false) => GetLights(client, name, allowPartialMatches).Where(light => group?.Lights.Contains(light.Id) ?? false).ToList();
        public static List<Light> GetLights(HueClient client, Scene scene, IProperty<string> name, bool allowPartialMatches = false) => GetLights(client, scene, name.FullValue, allowPartialMatches);
        public static List<Light> GetLights(HueClient client, Scene scene, string name = null, bool allowPartialMatches = false) => GetLights(client, name, allowPartialMatches).Where(light => scene?.Lights.Contains(light.Id) ?? false).ToList();
        public static List<Light> GetLights(HueClient client, IProperty<string> name, bool allowPartialMatches = false) => GetLights(client, name.FullValue, allowPartialMatches);
        public static List<Light> GetLights(HueClient client, string name = null, bool allowPartialMatches = false)
        {
            if (client != null)
            {
                var allLights = TryGet.ValueOrDefault(() => client.GetLightsAsync().Result, null);
                if (allLights != null)
                {
                    return string.IsNullOrEmpty(name) ? allLights.ToList() : allLights.Where(light => StringMatches(light.Name, name, allowPartialMatches)).ToList();
                }
            }

            return new List<Light>();
        }

        public static List<Sensor> GetSensors(HueClient client, IProperty<string> name, bool allowPartialMatches = false) => GetSensors(client, name.FullValue, allowPartialMatches);
        public static List<Sensor> GetSensors(HueClient client, string name = null, bool allowPartialMatches = false)
        {
            if (client != null)
            {
                var validSensors = (IEnumerable<Sensor>)TryGet.ValueOrDefault(() => client.GetSensorsAsync().Result, null).ToList();
                if (validSensors != null)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        validSensors = validSensors.Where(sensor => StringMatches(sensor.Name, name, allowPartialMatches));
                    }

                    return validSensors.ToList();
                }
            }

            return new List<Sensor>();
        }

        public static List<Sensor> GetCombinedSensors(HueClient client, Sensor sensor)
        {
            if (client != null && sensor != null)
            {
                var allSensors = GetSensors(client);
                if (allSensors != null)
                {
                    var endSection = sensor.UniqueId?.IndexOf('-') ?? -1;
                    if (endSection != -1)
                    {
                        var trimmedId = sensor.UniqueId.Substring(0, endSection);
                        if (!string.IsNullOrEmpty(trimmedId))
                        {
                            return allSensors.Where(sensor => sensor?.UniqueId?.Contains(trimmedId) ?? false).ToList();
                        }
                    }
                }

                return new List<Sensor>() { sensor };
            }

            return new List<Sensor>();
        }

        public static List<Sensor> GetMotionSensors(List<Sensor> sensors) => sensors?.Where(sensor => sensor?.State?.Presence != null).ToList();
        public static List<Sensor> GetTemperatureSensors(List<Sensor> sensors) => sensors?.Where(sensor => sensor?.State?.Temperature != null).ToList();
        public static List<Sensor> GetHumiditySensors(List<Sensor> sensors) => sensors?.Where(sensor => sensor?.State?.Humidity != null).ToList();
        public static List<Sensor> GetLightSensors(List<Sensor> sensors) => sensors?.Where(sensor => sensor?.State?.LightLevel != null).ToList();

        private static bool StringMatches(string @string, string match, bool allowPartialMatches = false) => @string != null && match != null && (allowPartialMatches ? @string.Contains(match) : @string == match);

        private static HueClient GenerateHueClient(string bridgeIp, string userId)
        {
            var key = $"{bridgeIp}|||{userId}";

            if (!_clientStorage.ContainsKey(key))
            {
                _clientStorage[key] = new LocalHueClient(bridgeIp, userId);
            }

            var client = _clientStorage[key];
            var httpClient = client.GetHttpClient().Result;
            if (httpClient != null)
            {
                httpClient.DefaultRequestHeaders.CacheControl = _cacheControl;
            }

            return client;
        }
    }
}
