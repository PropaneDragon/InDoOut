using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace InDoOut_UI_Common.InterfaceElements
{
    internal class CommonProgramLoader : AbstractCreator, ICommonProgramLoader
    {
        private readonly ICommonProgramDisplay _display = null;
        private readonly DispatcherTimer _wireRedrawTimer = null; //Todo: Look into making this safe.

        public CommonProgramLoader(ICommonProgramDisplay display)
        {
            _display = display;
            _wireRedrawTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromSeconds(2),
                IsEnabled = false
            };

            _wireRedrawTimer.Tick += WireRedrawTimer_Tick;
        }

        public bool DisplayProgram(IProgram program)
        {
            if (program != null && _display != null)
            {
                var functionToUIFunctionMap = new Dictionary<IFunction, IUIFunction>();

                if (_display is IScrollable scrollable)
                {
                    if (ExtractPointFromMetadata(program, out var location))
                    {
                        scrollable.Offset = location;
                    }
                    else
                    {
                        scrollable.MoveToCentre();
                    }
                }

                foreach (var function in program.Functions)
                {
                    var uiFunction = _display.Create(function);
                    if (uiFunction != null)
                    {
                        functionToUIFunctionMap.Add(function, uiFunction);
                    }
                }

                DisplayConnections(program, functionToUIFunctionMap);

                _wireRedrawTimer?.Start();
            }

            return false;
        }

        public bool UnloadProgram(IProgram program)
        {
            if (program != null)
            {
                program.Stop();

                return true;
            }

            return false;
        }

        private void DisplayConnections(IProgram program, Dictionary<IFunction, IUIFunction> functionToUIFunctionMap)
        {
            foreach (var function in program.Functions)
            {
                foreach (var output in function.Outputs)
                {
                    foreach (var input in output.Connections)
                    {
                        if (functionToUIFunctionMap.ContainsKey(function) && functionToUIFunctionMap.ContainsKey(input.Parent))
                        {
                            var startUiFunction = functionToUIFunctionMap[function];
                            var endUiFunction = functionToUIFunctionMap[input.Parent];

                            if (startUiFunction != null && endUiFunction != null)
                            {
                                var uiOutput = startUiFunction.Outputs.FirstOrDefault(uiOutput => uiOutput.AssociatedOutput == output);
                                var uiInput = endUiFunction.Inputs.FirstOrDefault(uiInput => uiInput.AssociatedInput == input);

                                if (uiOutput != null && uiInput != null)
                                {
                                    CreateConnection(uiOutput, uiInput, output, input);
                                }
                            }
                        }
                    }
                }

                foreach (var result in function.Results)
                {
                    foreach (var property in result.Connections)
                    {
                        if (functionToUIFunctionMap.ContainsKey(function) && functionToUIFunctionMap.ContainsKey(property.Parent))
                        {
                            var startUiFunction = functionToUIFunctionMap[function];
                            var endUiFunction = functionToUIFunctionMap[property.Parent];

                            if (startUiFunction != null && endUiFunction != null)
                            {
                                var uiResult = startUiFunction.Results.FirstOrDefault(uiResult => uiResult.AssociatedResult == result);
                                var uiProperty = endUiFunction.Properties.FirstOrDefault(uiProperty => uiProperty.AssociatedProperty == property);

                                if (uiResult != null && uiProperty != null)
                                {
                                    CreateConnection(uiResult, uiProperty, result, property);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateConnection(IUIConnectionStart uiStart, IUIConnectionEnd uiEnd, IStored start, IStored end)
        {
            if (uiStart != null && uiEnd != null && start != null && end != null)
            {
                var connection = _display.Create(uiStart, uiEnd);

                if (ExtractRectFromMetadata(start, out var startArea) && ExtractRectFromMetadata(end, out var endArea))
                {
                    var startCentre = startArea.TopLeft + ((startArea.BottomRight - startArea.TopLeft) / 2d);
                    var endCentre = endArea.TopLeft + ((endArea.BottomRight - endArea.TopLeft) / 2d);

                    connection.Start = _display.GetBestSide(startArea, endCentre);
                    connection.End = _display.GetBestSide(endArea, startCentre);
                }
                else
                {
                    _display.Remove(connection);
                }
            }
        }

        private void WireRedrawTimer_Tick(object sender, EventArgs e)
        {
            _wireRedrawTimer?.Stop();

            if (_display != null)
            {
                if (_display is FrameworkElement frameworkElement)
                {
                    frameworkElement.Measure(new Size(frameworkElement.ActualWidth, frameworkElement.ActualHeight));
                    frameworkElement.Arrange(new Rect(0, 0, frameworkElement.DesiredSize.Width, frameworkElement.DesiredSize.Height));
                }

                foreach (var uiConnection in _display.UIConnections)
                {
                    uiConnection?.UpdatePositionFromInputOutput(_display);
                }
            }
        }
    }
}
