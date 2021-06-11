using InDoOut_Core.Entities.Functions;
using System;
using System.Threading;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace InDoOut_Philips_Hue_Plugins
{
    public class CreateToastFunction : Function
    {
        private readonly IOutput _userDismissed, _timedOut, _appHidden, _activated, _error;
        private readonly IProperty<string> _title, _content;
        private readonly IProperty<double> _timeout;
        private readonly object _outputLock = new object();

        private IOutput _activatedOutput;

        private IOutput ActivatedOutput
        {
            get
            {
                lock (_outputLock)
                {
                    return _activatedOutput;
                }
            }
            set
            {
                lock (_outputLock)
                {
                    _activatedOutput = value;
                }
            }
        }

        public override string Description => "Creates a notification toast.";

        public override string Name => "Create notification toast";

        public override string Group => "Windows";

        public override string[] Keywords => new[] { "notification", "toast", "popup", "pop", "up", "overlay", "taskbar" };

        public override IOutput TriggerOnFailure => _error;

        public CreateToastFunction()
        {
            _ = CreateInput("Show notification");

            _activated = CreateOutput("Notification activated", OutputType.Positive);
            _userDismissed = CreateOutput("User dismissed", OutputType.Neutral);
            _timedOut = CreateOutput("Timed out", OutputType.Neutral);
            _appHidden = CreateOutput("App hidden", OutputType.Neutral);
            _error = CreateOutput("Notification error", OutputType.Negative);

            _title = AddProperty(new Property<string>("Title", "The title of the notification.", true, ""));
            _content = AddProperty(new Property<string>("Content", "The content of the notification.", true, ""));
            _timeout = AddProperty(new Property<double>("Timeout", "The amount of time before the notification disappears. Set to 0 for default timeout.", true, 0));
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            ActivatedOutput = null;

            var toastTemplate = $@"
                <toast>
                    <visual>
                    <binding template=""ToastText02"">
                        <text id=""1"">{_title?.FullValue ?? ""}</text>
                        <text id=""2"">{_content?.FullValue ?? ""}</text>
                    </binding>
                    </visual>
                </toast>";

            var xmlTemplate = new XmlDocument();
            xmlTemplate.LoadXml(toastTemplate);

            var notifier = ToastNotificationManager.CreateToastNotifier("InDoOut");
            var toastNotification = new ToastNotification(xmlTemplate);

            if ((_timeout?.FullValue ?? 0) > 0)
            {
                toastNotification.ExpirationTime = DateTimeOffset.Now.AddSeconds(_timeout.FullValue);
            }

            toastNotification.Activated += ToastNotification_Activated;
            toastNotification.Dismissed += ToastNotification_Dismissed;
            toastNotification.Failed += ToastNotification_Failed;

            notifier.Show(toastNotification);

            while (ActivatedOutput == null)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
            }

            toastNotification.Activated -= ToastNotification_Activated;
            toastNotification.Dismissed -= ToastNotification_Dismissed;
            toastNotification.Failed -= ToastNotification_Failed;

            return ActivatedOutput;
        }

        private IOutput GetOutputForDismissedReason(ToastDismissalReason reason)
        {
            return reason switch
            {
                ToastDismissalReason.ApplicationHidden => _appHidden,
                ToastDismissalReason.TimedOut => _timedOut,
                ToastDismissalReason.UserCanceled => _userDismissed,
                _ => null
            };
        }

        private void ToastNotification_Activated(ToastNotification sender, object args) => ActivatedOutput = _activated;

        private void ToastNotification_Dismissed(ToastNotification sender, ToastDismissedEventArgs args) => ActivatedOutput = GetOutputForDismissedReason(args.Reason);

        private void ToastNotification_Failed(ToastNotification sender, ToastFailedEventArgs args) => ActivatedOutput = _error;
    }
}
