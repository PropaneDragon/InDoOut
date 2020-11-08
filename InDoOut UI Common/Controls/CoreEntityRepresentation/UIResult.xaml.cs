using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Controls.CoreEntityRepresentation
{
    public partial class UIResult : UserControl, IUIResult
    {
        private IResult _result = null;
        private DispatcherTimer _valueUpdateTimer = null;

        public IResult AssociatedResult { get => _result; set => SetResult(value); }

        public UIResult() : base()
        {
            InitializeComponent();
        }

        public UIResult(IResult result) : this()
        {
            AssociatedResult = result;
        }

        public void PositionUpdated(Point position)
        {
            if (AssociatedResult != null)
            {
                AssociatedResult.Metadata["x"] = position.X.ToString();
                AssociatedResult.Metadata["y"] = position.Y.ToString();
                AssociatedResult.Metadata["w"] = ActualWidth.ToString();
                AssociatedResult.Metadata["h"] = ActualHeight.ToString();
            }
        }

        private void SetResult(IResult result)
        {
            if (_result != null)
            {
                //Todo: Teardown old property
            }

            _result = result;

            if (_result != null)
            {
                //Warning: Potential bug? INamed has no safe version.
                IO_Main.Text = _result.Name;
                IO_Main.Value = "";
            }
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            var propertyValue = await Task.Run(() => AssociatedResult?.RawValue ?? "");
            IO_Main.Value = propertyValue;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(0, 333)));

            _valueUpdateTimer = new DispatcherTimer(DispatcherPriority.Background)
            {
                Interval = TimeSpan.FromMilliseconds(333)
            };

            _valueUpdateTimer.Start();
            _valueUpdateTimer.Tick += UpdateTimer_Tick;

            IO_Main.Value = "updating...";
            IO_Main.Value = "";
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _valueUpdateTimer.Stop();
            _valueUpdateTimer.Tick -= UpdateTimer_Tick;
            _valueUpdateTimer = null;
        }
    }
}
