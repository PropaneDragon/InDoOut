using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Threading.Safety;
using Newtonsoft.Json.Linq;
using System;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ConnectToBridgeFunction : Function
    {
        private readonly IOutput _connected, _failed, _pressSyncButton;
        private readonly IProperty<string> _bridgeIp;
        private readonly IResult _userId;

        public override string Description => "Connects to a Hue bridge.";

        public override string Name => "Connect to bridge";

        public override string Group => "Philips Hue";

        public override string[] Keywords => new[] { "hue", "philips", "bridge", "connect", "sync", "connection", "create", "user", "api" };

        public ConnectToBridgeFunction()
        {
            _ = CreateInput("Connect");

            _connected = CreateOutput("Connected", OutputType.Positive);
            _failed = CreateOutput("Connection failed", OutputType.Negative);
            _pressSyncButton = CreateOutput("Press sync button");
            _bridgeIp = AddProperty(new Property<string>("Bridge IP", "The IP address of the bridge to connect to.", true));
            _userId = AddResult(new Result("User ID", "The user ID returned from the connection."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            dynamic user = new JObject();
            user.devicetype = "InDoOut#ConnectToBridgeFunction";

            if (!string.IsNullOrEmpty(_bridgeIp.FullValue))
            {
                dynamic result = TryGet.ValueOrDefault(() => JsonFromUrl.Instance.JsonArrayFromUrl(new Uri($"https://{_bridgeIp.FullValue}/api"), JsonFromUrl.Method.POST, user.ToString()).Result, null);
                if (result != null)
                {
                    var error = TryGet.ValueOrDefault(() => result[0].error, null);
                    var success = TryGet.ValueOrDefault(() => result[0].success, null);

                    if (error != null)
                    {
                        var errorType = TryGet.ValueOrDefault(() => error.type, 0);
                        if (errorType == 101 || errorType == "101")
                        {
                            return _pressSyncButton;
                        }
                    }
                    else if (success != null)
                    {
                        var userId = TryGet.ValueOrDefault(() => success.username, null);
                        if (!string.IsNullOrEmpty(userId.Value))
                        {
                            _userId.RawValue = userId;

                            return _connected;
                        }
                    }
                }
            }

            return _failed;
        }
    }
}
