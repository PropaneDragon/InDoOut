using InDoOut_Core.Entities.Functions;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using static InDoOut_Desktop.UI.Controls.CoreEntityRepresentation.UIFunctionIO;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIOutput : UserControl
    {
        private IOutput _output = null;

        public IOutput Output { get => _output; set => SetOutput(value); }

        public UIOutput()
        {
            InitializeComponent();
        }

        public UIOutput(IOutput output) : base()
        {
            Output = output;
        }

        private void SetOutput(IOutput input)
        {
            if (_output != null)
            {
                //Todo: Teardown old output
            }

            _output = input;

            if (_output != null)
            {
                //Warning: Potential bug? INamed has no safe version.
                IO_Main.Text = _output.Name;

                UpdateColourForOutput();
            }
        }

        private void UpdateColourForOutput()
        {
            var ioType = GetIOTypeForOutput();

            IO_Main.Type = ioType;
        }

        private IOType GetIOTypeForOutput()
        {
            if (Output is IOutputNegative) return IOType.Negative;
            if (Output is IOutputPositive) return IOType.Positive;
            if (Output is IOutputNeutral) return IOType.Neutral;

            return IOType.Neutral;
        }
    }
}
