using InDoOut_Core.Entities.Functions;
using System;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIInput : UserControl
    {
        private IInput _input = null;

        public IInput Input { get => _input; set => SetInput(value); }

        public UIInput()
        {
            InitializeComponent();
        }

        public UIInput(IInput input) : base()
        {
            Input = input;
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
