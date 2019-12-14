﻿using InDoOut_Display_Core.Functions;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_Display_Core.Elements
{
    /// <summary>
    /// A display element capable of being hosted on a screen and showing information to users.
    /// </summary>
    public abstract class DisplayElement : UserControl, IDisplayElement
    {
        private DispatcherTimer _updateTimer = null;

        /// <summary>
        /// The associated background function responsible for updating this element.
        /// </summary>
        public IElementFunction AssociatedElementFunction { get; private set; }

        private DisplayElement()
        {
            Loaded += UIElement_Loaded;
            Unloaded += UIElement_Unloaded;
        }

        /// <summary>
        /// Creates a new display element with an associated function.
        /// </summary>
        /// <param name="function">The function to associate with this element.</param>
        public DisplayElement(IElementFunction function) : this()
        {
            AssociatedElementFunction = function;
        }

        /// <summary>
        /// Called whenever an update has been requested by the <see cref="AssociatedElementFunction"/>. This should
        /// update this UI element to display information provided by the function.
        /// </summary>
        /// <param name="function">The function that requested the update.</param>
        /// <returns></returns>
        protected abstract bool UpdateRequested(IElementFunction function);

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (AssociatedElementFunction?.ShouldDisplayUpdate ?? false)
            {
                if (UpdateRequested(AssociatedElementFunction))
                {
                    AssociatedElementFunction.PerformedUIUpdate();
                }
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
