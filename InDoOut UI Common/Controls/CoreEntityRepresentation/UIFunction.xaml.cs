using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Functions;
using InDoOut_Core.Logging;
using InDoOut_UI_Common.Actions.Copying;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Controls.CoreEntityRepresentation
{
    public partial class UIFunction : UserControl, IUIFunction
    {
        private readonly List<IUIConnection> _cachedVisualConnections = new List<IUIConnection>();

        private DispatcherTimer _updateTimer = null;
        private DoubleAnimation _fadeAnimation = null;
        private UIFunctionDisplayMode _displayMode = UIFunctionDisplayMode.None;
        private IFunction _function = null;

        public IFunction AssociatedFunction { get => _function; set => SetFunction(value); }

        public List<IUIInput> Inputs => FindInCollection<IUIInput>(Stack_Inputs?.Children);

        public List<IUIOutput> Outputs => FindInCollection<IUIOutput>(Stack_Outputs?.Children);

        public List<IUIProperty> Properties => FindInCollection<IUIProperty>(Stack_Properties?.Children);

        public List<IUIResult> Results => FindInCollection<IUIResult>(Stack_Results?.Children);

        public UIFunctionDisplayMode DisplayMode { get => _displayMode; set => SetDisplayMode(value); }

        public UIFunction()
        {
            InitializeComponent();
        }

        public UIFunction(IFunction function) : this()
        {
            AssociatedFunction = function;
        }

        public bool CanSelect(ICommonDisplay display) => true;
        public bool CanDrag(ICommonDisplay display) => true;
        public bool CanCopy(ICommonDisplay display) => true;
        public bool CanDelete(ICommonDisplay display) => true;

        public void Deleted(ICommonDisplay display)
        {
            if (display != null && display is ICommonProgramDisplay programDisplay && (programDisplay.AssociatedProgram?.RemoveFunction(AssociatedFunction) ?? false))
            {
                _function?.PolitelyStop();

                if (!RemoveAllConnections(display))
                {
                    Log.Instance.Error($"Couldn't remove all connections from {this}");
                }
            }
        }

        public bool CopyTo(ICopyable other)
        {
            return other != null;
        }

        public ICopyable CreateCopy(ICommonDisplay display)
        {
            if (display != null && display is IFunctionDisplay functionDisplay && _function != null)
            {
                var functionBuilder = new FunctionBuilder();
                var functionInstance = functionBuilder.BuildInstance(_function.GetType());

                if (functionInstance != null)
                {
                    return functionDisplay?.FunctionCreator?.Create(functionInstance);
                }
            }

            return null;
        }

        public void SelectionStarted(ICommonDisplay display)
        {
            Rectangle_Selected.Visibility = Visibility.Visible;
        }

        public void SelectionEnded(ICommonDisplay display)
        {
            Rectangle_Selected.Visibility = Visibility.Hidden;
        }

        public void DragStarted(ICommonDisplay display)
        {
            _cachedVisualConnections.Clear();

            if (display != null && display is IConnectionDisplay connectionDisplay)
            {
                _cachedVisualConnections.AddRange(connectionDisplay.FindConnections(Inputs.Cast<IUIConnectionEnd>().ToList()));
                _cachedVisualConnections.AddRange(connectionDisplay.FindConnections(Outputs.Cast<IUIConnectionStart>().ToList()));
                _cachedVisualConnections.AddRange(connectionDisplay.FindConnections(Properties.Cast<IUIConnectionEnd>().ToList()));
                _cachedVisualConnections.AddRange(connectionDisplay.FindConnections(Results.Cast<IUIConnectionStart>().ToList()));
            }
        }

        public void DragMoved(ICommonDisplay display, Point delta)
        {
            if (AssociatedFunction != null)
            {
                var position = display.GetPosition(this);

                AssociatedFunction.Metadata["x"] = position.X.ToString();
                AssociatedFunction.Metadata["y"] = position.Y.ToString();
            }

            foreach (var cachedVisualConnection in _cachedVisualConnections)
            {
                cachedVisualConnection.UpdatePositionFromInputOutput(display);
            }
        }

        public void DragEnded(ICommonDisplay display)
        {
            _cachedVisualConnections.Clear();
        }

        private void SetFunction(IFunction function)
        {
            if (_function != null)
            {
                _function?.PolitelyStop();
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
                var running = (AssociatedFunction?.Running ?? false) || (AssociatedFunction?.HasBeenTriggeredWithin(TimeSpan.FromMilliseconds(200)) ?? false);

                if ((running && Text_Processing.Opacity <= 0.01) || (!running && Text_Processing.Opacity >= 0.99))
                {
                    _fadeAnimation = new DoubleAnimation(running ? 1 : 0, TimeSpan.FromMilliseconds(running ? 200 : 300)) { EasingFunction = new CircleEase() { EasingMode = EasingMode.EaseInOut } };

                    Text_Processing.BeginAnimation(OpacityProperty, _fadeAnimation);
                }

                Text_FunctionName.Text = AssociatedFunction.SafeName;
                Text_Processing.Visibility = Visibility.Visible;
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

        private bool RemoveAllConnections(ICommonDisplay display)
        {
            var allDeleted = true;
            allDeleted = RemoveEndConnections(Inputs, display) && allDeleted;
            allDeleted = RemoveEndConnections(Properties, display) && allDeleted;
            allDeleted = RemoveStartConnections(Outputs, display) && allDeleted;
            allDeleted = RemoveStartConnections(Results, display) && allDeleted;

            return allDeleted;
        }

        private bool RemoveStartConnections<StartType>(List<StartType> start, ICommonDisplay display) where StartType : IUIConnectionStart => display is IConnectionDisplay connectionDisplay ? RemoveConnections(connectionDisplay.FindConnections(start.Cast<IUIConnectionStart>().ToList()), display) : false;

        private bool RemoveEndConnections<EndType>(List<EndType> end, ICommonDisplay display) where EndType : IUIConnectionEnd => display is IConnectionDisplay connectionDisplay ? RemoveConnections(connectionDisplay.FindConnections(end.Cast<IUIConnectionEnd>().ToList()), display) : false;

        private bool RemoveConnections(List<IUIConnection> connections, ICommonDisplay display)
        {
            var allDeleted = true;

            foreach (var connection in connections)
            {
                allDeleted = connection.CanDelete(display) && (display?.DeletableRemover?.Remove(connection) ?? false);
            }

            return allDeleted;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdateFromFunction();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _updateTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };

            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
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

        private void UserControl_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }
    }
}
