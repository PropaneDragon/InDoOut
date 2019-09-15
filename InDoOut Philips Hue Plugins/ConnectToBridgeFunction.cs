using InDoOut_Core.Entities.Functions;
using Q42.HueApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace InDoOut_Philips_Hue_Plugins
{
    public class ConnectToBridgeFunction : Function
    {
        private static readonly string FILENAME_BRIDGE_USER_IDS = "bridge_ids.txt";

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
                if (!string.IsNullOrWhiteSpace(_bridgeIp.FullValue))
                {
                    var idLookup = $"ClientId#{_bridgeIp.FullValue}#: ";
                    var client = new LocalHueClient(_bridgeIp.FullValue);

                    if (File.Exists(FILENAME_BRIDGE_USER_IDS))
                    {
                        var lines = File.ReadAllLines(FILENAME_BRIDGE_USER_IDS);

                        foreach (var line in lines)
                        {
                            if (line.Contains(idLookup))
                            {
                                var potentialClientId = line.Replace(idLookup, "");
                                if (!string.IsNullOrWhiteSpace(potentialClientId))
                                {
                                    client.Initialize(potentialClientId);

                                    if (client.IsInitialized && _userId.ValueFrom(potentialClientId))
                                    {
                                        return _connected;
                                    }
                                }
                            }
                        }
                    }

                    var clientId = client.RegisterAsync("InDoOut", "Function").Result;
                    if (!string.IsNullOrWhiteSpace(clientId))
                    {
                        File.AppendAllLines(FILENAME_BRIDGE_USER_IDS, new List<string>() { $"{idLookup}{clientId}" });

                        if (_userId.ValueFrom(clientId))
                        {
                            return _connected;
                        }
                    }
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
