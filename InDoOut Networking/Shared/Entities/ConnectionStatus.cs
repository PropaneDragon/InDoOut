using InDoOut_Json_Storage;
using Newtonsoft.Json;

namespace InDoOut_Networking.Shared.Entities
{
    [JsonObject("connectionStatus")]
    public class ConnectionStatus : JsonConnection
    {
        public ConnectionStatus(JsonConnection baseConnection)
        {
            if (baseConnection != null)
            {
                TypeOfConnection = baseConnection.TypeOfConnection;
                OutputName = baseConnection.OutputName;
                InputName = baseConnection.InputName;
                StartFunctionId = baseConnection.StartFunctionId;
                EndFunctionId = baseConnection.EndFunctionId;
                InputMetadata = baseConnection.InputMetadata;
                OutputMetadata = baseConnection.OutputMetadata;
            }
        }

        public static ConnectionStatus FromJson(string json)
        {
            try
            {
                var generatedStatus = JsonConvert.DeserializeObject<ConnectionStatus>(json);
                return generatedStatus;
            }
            catch { }

            return null;
        }

        public string ToJson()
        {
            try
            {
                var generatedText = JsonConvert.SerializeObject(this);
                return generatedText;
            }
            catch { }

            return null;
        }
    }
}
