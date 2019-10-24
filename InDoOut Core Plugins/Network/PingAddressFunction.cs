using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Logging;
using System;
using System.Net.NetworkInformation;

namespace InDoOut_Core_Plugins.Network
{
    public class PingAddressFunction : Function
    {
        private IOutput _success, _noResponse, _error;
        private IProperty<string> _address;
        private IProperty<double> _timeout;
        private IResult _replyAddress, _roundTripTime;

        public override string Description => "Pings a network address to see whether a response is given. Useful for checking if a device is active on the network.";

        public override string Name => "Ping address";

        public override string Group => "Network";

        public override string[] Keywords => new[] { "pong", "networking", "reply", "message", "ICMP", "internet control message protocol", "internet", "control", "protocol" };

        public override IOutput TriggerOnFailure => _error;

        public PingAddressFunction()
        {
            _ = CreateInput("Ping");

            _success = CreateOutput("Success", OutputType.Positive);
            _noResponse = CreateOutput("No response", OutputType.Negative);
            _error = CreateOutput("Error", OutputType.Negative);

            _address = AddProperty(new Property<string>("Address", "The address to ping", true));
            _timeout = AddProperty(new Property<double>("Timeout (milliseconds)", "The amount of time to wait before giving up", false, 1000));

            _replyAddress = AddResult(new Result("Response address", "The IP address returned from the response (if successful)."));
            _roundTripTime = AddResult(new Result("Round trip time", "The amount of time it took to send and get a response from the device."));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            using var ping = new Ping();

            try
            {
                var replyTask = ping.SendPingAsync(_address.FullValue, (int)Math.Round(_timeout.FullValue));
                if (replyTask != null)
                {
                    var reply = replyTask.Result;
                    if (reply != null)
                    {
                        Log.Instance.Info("Ping result: ", reply?.Status);

                        _replyAddress.ValueFrom(reply.Address.ToString());
                        _roundTripTime.ValueFrom(reply.RoundtripTime);

                        return reply.Status switch
                        {
                            IPStatus.TimedOut => _noResponse,
                            IPStatus.TimeExceeded => _noResponse,
                            IPStatus.Success => _success,

                            _ => _error
                        };
                    }
                }
            }
            catch { }

            return _error;
        }
    }
}
