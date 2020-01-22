using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Controls.CoreEntityRepresentation
{
    public partial class UIInput : UserControl, IUIInput
    {
        private IInput _input = null;

        public IInput AssociatedInput { get => _input; set => SetInput(value); }

        public UIInput() : base()
        {
            InitializeComponent();
        }

        public UIInput(IInput input) : this()
        {
            AssociatedInput = input;
        }

        public void PositionUpdated(Point position)
        {
            if (AssociatedInput != null)
            {
                AssociatedInput.Metadata["x"] = position.X.ToString();
                AssociatedInput.Metadata["y"] = position.Y.ToString();
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
                //Warning: Potential bug? INamed has no safe version.
                IO_Main.Text = _input.Name;
            }
        }
    }
}
