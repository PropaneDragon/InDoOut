using InDoOut_Display_Core.Functions;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_Display_Core.Elements
{
    public abstract class DisplayElement : UserControl, IDisplayElement
    {
        private DispatcherTimer _updateTimer = null;

        public IElementFunction AssociatedElement { get; private set; }

        private DisplayElement()
        {
            Loaded += UIElement_Loaded;
            Unloaded += UIElement_Unloaded;
        }

        public DisplayElement(IElementFunction element) : this()
        {
            AssociatedElement = element;
        }

        protected abstract bool UpdateRequested(IElementFunction element);

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (AssociatedElement?.ShouldDisplayUpdate ?? false)
            {
                UpdateRequested(AssociatedElement);
                AssociatedElement.PerformedUIUpdate();
            }
        }

        private void UIElement_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_updateTimer == null)
            {
                _updateTimer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromMilliseconds(100),
                    IsEnabled = true
                };

                _updateTimer.Tick += UpdateTimer_Tick;
            }
        }

        private void UIElement_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_updateTimer != null)
            {
                _updateTimer.Stop();
                _updateTimer.Tick -= UpdateTimer_Tick;
                _updateTimer = null;
            }
        }
    }
}
