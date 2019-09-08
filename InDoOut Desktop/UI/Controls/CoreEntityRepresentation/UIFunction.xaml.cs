using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.Actions;
using InDoOut_Desktop.UI.Interfaces;
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
        private UIFunctionDisplayMode _displayMode = UIFunctionDisplayMode.None;
        private IFunction _function = null;
        private readonly List<IUIConnection> _cachedVisualConnections = new List<IUIConnection>();

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

        public bool CanSelect(IBlockView view) => true;
        public bool CanDrag(IBlockView view) => true;

        public void SelectionStarted(IBlockView view)
        {
            Rectangle_Selected.Visibility = Visibility.Visible;
        }

        public void SelectionEnded(IBlockView view)
        {
            Rectangle_Selected.Visibility = Visibility.Hidden;
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
            if (AssociatedFunction != null)
            {
                var position = view.GetPosition(this);

                AssociatedFunction.Metadata["x"] = position.X.ToString();
                AssociatedFunction.Metadata["y"] = position.Y.ToString();
            }

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
                        _ = panel.Children.Add(element);
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

        private void SetDisplayMode(UIFunctionDisplayMode displayMode)
        {
            if (displayMode != _displayMode)
            {
                var animationTime = _displayMode != UIFunctionDisplayMode.None ? TimeSpan.FromMilliseconds(200) : TimeSpan.FromMilliseconds(1);

                var outAnimation = new DoubleAnimation(0, animationTime) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseIn } };
                var inAnimation = new DoubleAnimation(1, animationTime) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseOut } };

                var ioElements = new List<UIElement>() { Stack_Inputs, Stack_Outputs };
                var variableElements = new List<UIElement>() { Stack_Properties, Stack_Results };

                var elementsToHide = displayMode == UIFunctionDisplayMode.IO ? variableElements : ioElements;
                var elementsToShow = displayMode == UIFunctionDisplayMode.IO ? ioElements : variableElements;

                outAnimation.Completed += (sender, e) =>
                {
                    foreach (var elementToHide in elementsToHide)
                    {
                        var transformGroup = new TransformGroup();
                        var scaleTransform = new ScaleTransform(1, 1);

                        transformGroup.Children.Add(scaleTransform);

                        elementToHide.RenderTransform = transformGroup;
                        elementToHide.Visibility = Visibility.Hidden;
                    }

                    foreach (var elementToShow in elementsToShow)
                    {
                        var transformGroup = new TransformGroup();
                        var scaleTransform = new ScaleTransform(1, 0);

                        transformGroup.Children.Add(scaleTransform);

                        elementToShow.RenderTransform = transformGroup;
                        elementToShow.Visibility = Visibility.Visible;

                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, inAnimation);
                    }
                };

                foreach (var elementToHide in elementsToHide)
                {
                    var transformGroup = new TransformGroup();
                    var scaleTransform = new ScaleTransform(1, 1);

                    transformGroup.Children.Add(scaleTransform);

                    elementToHide.RenderTransform = transformGroup;

                    scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, outAnimation);
                }
            }

            _displayMode = displayMode;
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
            fadeOutAnimation.Completed += (sender, e) => Button_Run.Visibility = Visibility.Hidden;

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
