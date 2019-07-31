using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.UI.Interfaces;
using System.Windows.Controls;
using static InDoOut_Desktop.UI.Controls.CoreEntityRepresentation.UIFunctionIO;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIOutput : UserControl, IUIOutput
    {
        private IOutput _output = null;

        public IOutput AssociatedOutput { get => _output; set => SetOutput(value); }

        public UIOutput() : base()
        {
            InitializeComponent();
        }

        public UIOutput(IOutput output) : this()
        {
            AssociatedOutput = output;
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
            if (AssociatedOutput is IOutputNegative) return IOType.Negative;
            if (AssociatedOutput is IOutputPositive) return IOType.Positive;
            if (AssociatedOutput is IOutputNeutral) return IOType.Neutral;

            return IOType.Neutral;
        }
    }
}
