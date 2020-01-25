using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class DisplayElementOutput : UserControl, IUIOutput
    {
        private IOutput _output = null;

        public IOutput AssociatedOutput { get => _output; set => SetOutput(value); }

        public DisplayElementOutput()
        {
            InitializeComponent();
        }

        public DisplayElementOutput(IOutput output) : this()
        {
            AssociatedOutput = output;
        }

        public void PositionUpdated(Point point)
        {
            if (AssociatedOutput != null)
            {
                AssociatedOutput.Metadata["x"] = point.X.ToString();
                AssociatedOutput.Metadata["y"] = point.Y.ToString();
                AssociatedOutput.Metadata["w"] = ActualWidth.ToString();
                AssociatedOutput.Metadata["h"] = ActualHeight.ToString();
            }
        }

        private void SetOutput(IOutput output)
        {
            if (_output != null)
            {
                //Todo: Teardown old output
            }

            _output = output;

            if (_output != null)
            {
                //Todo: Set up from new output?
            }
        }
    }
}
