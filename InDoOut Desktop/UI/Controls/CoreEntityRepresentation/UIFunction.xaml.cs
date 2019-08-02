using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIFunction : UserControl, IDraggable, IUIFunction
    {
        private DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);
        private IFunction _function = null;
        private List<IUIConnection> _cachedVisualConnections = new List<IUIConnection>();

        public IFunction AssociatedFunction { get => _function; set => SetFunction(value); }

        public List<IUIInput> Inputs => FindInputs();

        public List<IUIOutput> Outputs => FindOutputs();

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

        public void DragStarted(IBlockView view)
        {
            _cachedVisualConnections.Clear();

            if (view != null)
            {
                _cachedVisualConnections.AddRange(view.FindConnections(Inputs));
                _cachedVisualConnections.AddRange(view.FindConnections(Outputs));
            }
        }

        public void DragMoved(IBlockView view)
        {
            foreach (var cachedVisualConnection in _cachedVisualConnections)
            {
                cachedVisualConnection.UpdatePositionFromInputOutput(view);
            }
        }

        public void DragEnded(IBlockView view)
        {
            _cachedVisualConnections.Clear();
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

        private List<IUIInput> FindInputs()
        {
            var foundInputs = new List<IUIInput>();
            var inputStackElements = Stack_Inputs.Children;

            foreach (var element in inputStackElements)
            {
                if (element is IUIInput input)
                {
                    foundInputs.Add(input);
                }
            }

            return foundInputs;
        }

        private List<IUIOutput> FindOutputs()
        {
            var foundOutputs = new List<IUIOutput>();
            var outputStackElements = Stack_Outputs.Children;

            foreach (var element in outputStackElements)
            {
                if (element is IUIOutput output)
                {
                    foundOutputs.Add(output);
                }
            }

            return foundOutputs;
        }

        private void UpdateOutputs()
        {
            if (AssociatedFunction != null)
            {
                var expectedOutputs = new List<IOutput>(AssociatedFunction.Outputs);
                var currentOutputs = Outputs;

                foreach (var output in currentOutputs)
                {
                    var associatedOutput = output.AssociatedOutput;

                    if (expectedOutputs.Contains(associatedOutput))
                    {
                        expectedOutputs.Remove(associatedOutput);
                    }
                    else if (output is UIElement element)
                    {
                        Stack_Outputs.Children.Remove(element);
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
                var currentInputs = Inputs;

                foreach (var input in currentInputs)
                {
                    var associatedInput = input.AssociatedInput;

                    if (expectedInputs.Contains(associatedInput))
                    {
                        expectedInputs.Remove(associatedInput);
                    }
                    else if (input is UIElement element)
                    {
                        Stack_Inputs.Children.Remove(element);
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
