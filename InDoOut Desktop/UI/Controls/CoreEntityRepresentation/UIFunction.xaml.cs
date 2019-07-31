using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIFunction : UserControl, IDraggable, IUIFunction
    {
        private DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);
        private IFunction _function = null;

        public IFunction AssociatedFunction { get => _function; set => SetFunction(value); }

        public UIFunction()
        {
            InitializeComponent();

            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        public UIFunction(IFunction function) : this()
        {
            AssociatedFunction = function;
        }

        public bool CanDrag()
        {
            return true;
        }

        public void DragStarted()
        {
        }

        public void DragMoved()
        {
        }

        public void DragEnded()
        {
        }

        private void SetFunction(IFunction function)
        {
            if (_function != null)
            {
                //Todo: Teardown from old function
            }

            _function = function;

            if (_function != null)
            {
                UpdateFromFunction();
                CreateInputs();
                CreateOutputs();
            }
        }

        private void UpdateFromFunction()
        {
            if (AssociatedFunction != null)
            {
                Text_FunctionName.Text = AssociatedFunction.SafeName;

                /*UpdateInputs();
                UpdateOutputs();*/
            }
        }

        private void CreateOutputs()
        {
            if (AssociatedFunction != null)
            {
                Stack_Outputs.Children.Clear();

                foreach (var output in AssociatedFunction.Outputs)
                {
                    var uiOutput = new UIOutput(output);

                    Stack_Outputs.Children.Add(uiOutput);
                }
            }
        }

        private void CreateInputs()
        {
            if (AssociatedFunction != null)
            {
                Stack_Inputs.Children.Clear();

                foreach (var input in AssociatedFunction.Inputs)
                {
                    var uiInput = new UIInput(input);

                    Stack_Inputs.Children.Add(uiInput);
                }
            }
        }

        private void UpdateOutputs()
        {
            if (AssociatedFunction != null)
            {
                var expectedOutputs = new List<IOutput>(AssociatedFunction.Outputs);
                var currentUIOutputs = Stack_Outputs.Children;

                foreach (var output in currentUIOutputs)
                {
                    if (output is UIOutput uiOutput)
                    {
                        var associatedOutput = uiOutput.AssociatedOutput;
                        if (expectedOutputs.Contains(associatedOutput))
                        {
                            expectedOutputs.Remove(associatedOutput);
                        }
                        else
                        {
                            Stack_Outputs.Children.Remove(uiOutput);
                        }
                    }
                }

                foreach (var output in expectedOutputs)
                {
                    var uiOutput = new UIOutput(output);

                    Stack_Outputs.Children.Add(uiOutput);
                }
            }
        }

        private void UpdateInputs()
        {
            if (AssociatedFunction != null)
            {
                var expectedInputs = new List<IInput>(AssociatedFunction.Inputs);
                var currentUIInputs = Stack_Inputs.Children;

                foreach (var input in currentUIInputs)
                {
                    if (input is UIInput uiInput)
                    {
                        var associatedInput = uiInput.AssociatedInput;
                        if (expectedInputs.Contains(associatedInput))
                        {
                            expectedInputs.Remove(associatedInput);
                        }
                        else
                        {
                            Stack_Inputs.Children.Remove(uiInput);
                        }
                    }
                }

                foreach (var input in expectedInputs)
                {
                    var uiInput = new UIInput(input);

                    Stack_Inputs.Children.Add(uiInput);
                }
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateFromFunction();
        }

        private void UserControl_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            _updateTimer.Tick -= UpdateTimer_Tick;
            _updateTimer.Stop();
            _updateTimer = null;
        }
    }
}
