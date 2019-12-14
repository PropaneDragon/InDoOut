﻿using InDoOut_Display_Core.Functions;
using InDoOut_Display_Plugins.Containers;
using InDoOut_Plugins.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Threading;

namespace InDoOut_Display.UI.Controls.ElementSelector
{
    public partial class ElementSelector : UserControl
    {
        private readonly TimeSpan _slowLoadSpeed = TimeSpan.FromMilliseconds(10);
        private readonly IElementFunctionBuilder _functionBuilder = new ElementFunctionBuilder();
        private int _lastElementId = 0;
        private DispatcherTimer _slowElementLoader = null;
        private IEnumerable<Type> _pluginTypes = null;

        public ElementSelector()
        {
            InitializeComponent();
        }

        public void LoadElementsSlowly()
        {
            KillElementLoader();

            Wrap_Elements.Children.Clear();

            _lastElementId = 0;
            _slowElementLoader = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = _slowLoadSpeed,
                IsEnabled = true
            };

            var plugins = LoadedPlugins.Instance.Plugins;
            _pluginTypes = plugins.Where(pluginContainer => pluginContainer is IElementPluginContainer).Cast<IElementPluginContainer>().SelectMany(plugin => plugin.ElementFunctionTypes).Distinct();

            _slowElementLoader.Tick += SlowElementLoader_Tick;
        }

        private void KillElementLoader()
        {
            if (_slowElementLoader != null)
            {
                _slowElementLoader.Stop();
                _slowElementLoader.Tick -= SlowElementLoader_Tick;
                _slowElementLoader = null;
            }
        }

        private void CreateNewElementSelectionFromFunction(IElementFunction function)
        {
            if (function != null)
            {
                var item = new ElementItem();
                if (item.LoadElementFromFunction(function))
                {
                    _ = Wrap_Elements.Children.Add(item);
                }
            }
        }

        private void SlowElementLoader_Tick(object sender, EventArgs e)
        {
            _slowElementLoader.Stop();

            var currentElementId = _lastElementId++;

            if (_pluginTypes != null && currentElementId < _pluginTypes.Count())
            {
                var currentPluginType = _pluginTypes.ElementAt(currentElementId);
                if (currentPluginType != null && _functionBuilder != null)
                {
                    var elementFunction = _functionBuilder.BuildInstance(currentPluginType);
                    if (elementFunction != null)
                    {
                        CreateNewElementSelectionFromFunction(elementFunction);
                    }
                }

                _slowElementLoader.Start();
            }
            else
            {
                KillElementLoader();
            }
        }

        private void UserControl_Initialized(object sender, System.EventArgs e)
        {
            LoadElementsSlowly();
        }
    }
}
