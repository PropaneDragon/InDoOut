using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.UI.Interfaces;
using InDoOut_Desktop.UI.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIFunction : UserControl, IDraggable, IUIFunction
    {
        private DispatcherTimer _updateTimer = new DispatcherTimer(DispatcherPriority.Normal);
        private UIFunctionDisplayMode _displayMode = UIFunctionDisplayMode.IO;
        private IFunction _function = null;
        private List<IUIConnection> _cachedVisualConnections = new List<IUIConnection>();

        public IFunction AssociatedFunction { get => _function; set => SetFunction(value); }

        public List<IUIInput> Inputs => FindInCollection<IUIInput>(Stack_Inputs?.Children);

        public List<IUIOutput> Outputs => FindInCollection<IUIOutput>(Stack_Outputs?.Children);

        public List<IUIProperty> Properties => FindInCollection<IUIProperty>(Stack_Properties?.Children);

        public List<IUIResult> Results => FindInCollection<IUIResult>(Stack_Results?.Children);

        public UIFunctionDisplayMode DisplayMode { get => _displayMode; set => SetDisplayMode(value); }

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
                _cachedVisualConnections.AddRange(view.FindConnections(Inputs.Cast<IUIConnectionEnd>().ToList()));
                _cachedVisualConnections.AddRange(view.FindConnections(Outputs.Cast<IUIConnectionStart>().ToList()));
                _cachedVisualConnections.AddRange(view.FindConnections(Properties.Cast<IUIConnectionEnd>().ToList()));
                _cachedVisualConnections.AddRange(view.FindConnections(Results.Cast<IUIConnectionStart>().ToList()));
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
                CreateProperties();
                CreateResults();
            }
        }

        private void UpdateFromFunction()
        {
            if (AssociatedFunction != null)
            {
                Text_FunctionName.Text = AssociatedFunction.SafeName;
                Text_Processing.Visibility = AssociatedFunction.Running ? Visibility.Visible : Visibility.Hidden;

                /*UpdateInputs();
                UpdateOutputs();*/
            }
        }

        private void CreateOutputs() => SetStackFromInterfaces(AssociatedFunction?.Outputs, Stack_Outputs, (output) => new UIOutput(output));
        private void CreateInputs() => SetStackFromInterfaces(AssociatedFunction?.Inputs, Stack_Inputs, (input) => new UIInput(input));
        private void CreateProperties() => SetStackFromInterfaces(AssociatedFunction?.Properties, Stack_Properties, (property) => new UIProperty(property));
        private void CreateResults() => SetStackFromInterfaces(AssociatedFunction?.Results, Stack_Results, (result) => new UIResult(result));

        private void SetStackFromInterfaces<Element, Interface>(List<Interface> interfaces, StackPanel panel, Func<Interface, Element> creationFunction) where Element : UIElement where Interface : class
        {
            if (interfaces != null && creationFunction != null && panel != null)
            {
                panel.Children.Clear();

                foreach (var @interface in interfaces)
                {
                    var element = creationFunction(@interface);
                    if (element != null)
                    {
                        panel.Children.Add(element);
                    }
                }
            }
        }

        private List<T> FindInCollection<T>(UIElementCollection collection) where T : class
        {
            var foundElements = new List<T>();

            if (collection != null)
            {
                foreach (var element in collection)
                {
                    if (element is T elementAsType)
                    {
                        foundElements.Add(elementAsType);
                    }
                }
            }

            return foundElements;
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

        private void SetDisplayMode(UIFunctionDisplayMode displayMode)
        {
            Stack_Inputs.Visibility = displayMode == UIFunctionDisplayMode.IO ? Visibility.Visible : Visibility.Hidden;
            Stack_Outputs.Visibility = displayMode == UIFunctionDisplayMode.IO ? Visibility.Visible : Visibility.Hidden;
            Stack_Properties.Visibility = displayMode == UIFunctionDisplayMode.Variables ? Visibility.Visible : Visibility.Hidden;
            Stack_Results.Visibility = displayMode == UIFunctionDisplayMode.Variables ? Visibility.Visible : Visibility.Hidden;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateFromFunction();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _updateTimer.Tick -= UpdateTimer_Tick;
            _updateTimer.Stop();
            _updateTimer = null;
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeInAnimation = new DoubleAnimation(1d, TimeSpan.FromMilliseconds(100));

            Button_Run.Visibility = Visibility.Visible;
            Button_Run.Opacity = 0;
            Button_Run.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var fadeOutAnimation = new DoubleAnimation(0d, TimeSpan.FromMilliseconds(100));
            fadeOutAnimation.Completed += (sender, e) => { Button_Run.Visibility = Visibility.Hidden; };

            Button_Run.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            var scaleAnimation = new DoubleAnimation(1d, TimeSpan.FromMilliseconds(400)) { EasingFunction = new BackEase() { EasingMode = EasingMode.EaseOut } };

            Scale_Main.ScaleX = 0;
            Scale_Main.ScaleY = 0;

            Scale_Main.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            Scale_Main.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }

        private void Button_Run_Click(object sender, RoutedEventArgs e)
        {
            _function?.Trigger(null);
        }
    }
}
