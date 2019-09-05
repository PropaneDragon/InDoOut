using InDoOut_Core.Entities.Functions;
using Q42.HueApi;
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
            try
            {
                var client = new LocalHueClient(_bridgeIp.FullValue);
                if (_userId.ValueFrom(client.RegisterAsync("InDoOut", "Remote").Result))
                {
                    return _connected;
                }
            }
            catch (AggregateException ex)
            {
                return _pressSyncButton;
            }
            catch (LinkButtonNotPressedException)
            {
                return _pressSyncButton;
            }
            catch { }

            return _failed;
        }
    }
}
