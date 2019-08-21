﻿using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.UI.Interfaces;
using System.Windows;
using System.Windows.Controls;

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

        public void PositionUpdated(Point position)
        {
            if (AssociatedOutput != null)
            {
                AssociatedOutput.Metadata["x"] = position.X.ToString();
                AssociatedOutput.Metadata["y"] = position.Y.ToString();
                AssociatedOutput.Metadata["w"] = ActualWidth.ToString();
                AssociatedOutput.Metadata["h"] = ActualHeight.ToString();
            }
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

        private UIFunctionIO.IOType GetIOTypeForOutput()
        {
            if (AssociatedOutput is IOutputNegative) return UIFunctionIO.IOType.Negative;
            if (AssociatedOutput is IOutputPositive) return UIFunctionIO.IOType.Positive;

            return UIFunctionIO.IOType.Neutral;
        }
    }
}
