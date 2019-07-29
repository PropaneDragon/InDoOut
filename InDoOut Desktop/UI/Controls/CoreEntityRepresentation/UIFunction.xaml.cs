using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIFunction : UserControl, IDraggable
    {
        private DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);
        private IFunction _function = null;

        public IFunction Function { get => _function; set => SetFunction(value); }

        public UIFunction()
        {
            InitializeComponent();

            _updateTimer.Interval = TimeSpan.FromMilliseconds(100);
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        public UIFunction(IFunction function) : this()
        {
            Function = function;
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
            }
        }

        private void UpdateFromFunction()
        {
            if (Function != null)
            {
                Text_FunctionName.Text = Function.SafeName;

                UpdateInputs();
                UpdateOutputs();
            }
        }

        private void UpdateOutputs()
        {
            if (Function != null)
            {
                Stack_Outputs.Children.Clear();

                foreach (var output in Function.Outputs)
                {
                    var uiOutput = new UIOutput();

                    Stack_Outputs.Children.Add(uiOutput);
                }
            }
        }

        private void UpdateInputs()
        {
            if (Function != null)
            {
                Stack_Inputs.Children.Clear();

                foreach (var input in Function.Inputs)
                {
                    var uiInput = new UIInput();

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
