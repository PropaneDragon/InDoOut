using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.DisplayElement
{
    public partial class DisplayElementInput : UserControl, IUIInput
    {
        private IInput _input = null;

        public IInput AssociatedInput { get => _input; set => SetInput(value); }

        public DisplayElementInput()
        {
            InitializeComponent();
        }

        public DisplayElementInput(IInput input) : this()
        {
            AssociatedInput = input;
        }

        public void PositionUpdated(Point point)
        {
            if (AssociatedInput != null)
            {
                AssociatedInput.Metadata["x"] = point.X.ToString();
                AssociatedInput.Metadata["y"] = point.Y.ToString();
                AssociatedInput.Metadata["w"] = ActualWidth.ToString();
                AssociatedInput.Metadata["h"] = ActualHeight.ToString();
            }
        }

        private void SetInput(IInput input)
        {
            if (_input != null)
            {
                //Todo: Teardown old input
            }

            _input = input;

            if (_input != null)
            {
                //Todo: Set up from new input?
            }
        }
    }
}
