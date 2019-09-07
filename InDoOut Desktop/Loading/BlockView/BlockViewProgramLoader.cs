using InDoOut_Core.Basic;
using InDoOut_Core.Entities.Functions;
using InDoOut_Core.Entities.Programs;
using InDoOut_Desktop.UI.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace InDoOut_Desktop.Loading.BlockView
{
    internal class BlockViewProgramLoader
    {
        private readonly IBlockView _associatedBlockView = null;

        public BlockViewProgramLoader(IBlockView blockView)
        {
            _associatedBlockView = blockView;
        }

        public bool DisplayProgram(IProgram program)
        {
            if (program != null && _associatedBlockView != null)
            {
                var functionToUIFunctionMap = new Dictionary<IFunction, IUIFunction>();

                if (ExtractLocation(program, out var location) && _associatedBlockView is IScrollable scrollable)
                {
                    scrollable.Offset = location;
                }

                foreach (var function in program.Functions)
                {
                    var uiFunction = ExtractLocation(function, out var functionLocation) ? _associatedBlockView.Create(function, functionLocation) : _associatedBlockView.Create(function);
                    if (uiFunction != null)
                    {
                        functionToUIFunctionMap.Add(function, uiFunction);
                    }
                }

                DisplayConnections(program, functionToUIFunctionMap);
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
                var connection = _associatedBlockView.Create(uiStart, uiEnd);

                if (ExtractArea(start, out var startArea) && ExtractArea(end, out var endArea))
                {
                    var startCentre = startArea.TopLeft + ((startArea.BottomRight - startArea.TopLeft) / 2d);
                    var endCentre = endArea.TopLeft + ((endArea.BottomRight - endArea.TopLeft) / 2d);

                    connection.Start = _associatedBlockView.GetBestSide(startArea, endCentre);
                    connection.End = _associatedBlockView.GetBestSide(endArea, startCentre);
                }
                else
                {
                    _associatedBlockView.Remove(connection);
                }
            }
        }

        public bool UnloadProgram(IProgram program)
        {
            if (program != null)
            {
                return true;
            }

            return false;
        }

        private bool ExtractLocation(IStored stored, out Point location)
        {
            location = new Point();

            if (stored != null && ExtractMetadataValue(stored, "x", out var x) && ExtractMetadataValue(stored, "y", out var y))
            {
                location = new Point(x, y);

                return true;
            }

            return false;
        }

        private bool ExtractArea(IStored stored, out Rect area)
        {
            area = new Rect();

            if (stored != null && ExtractLocation(stored, out var location) && ExtractMetadataValue(stored, "w", out var width) && ExtractMetadataValue(stored, "h", out var height))
            {
                area = new Rect(location, new Size(width, height));

                return true;
            }

            return false;
        }

        private bool ExtractMetadataValue(IStored stored, string key, out double value)
        {
            value = 0;
            return !string.IsNullOrEmpty(key) && stored != null && stored.Metadata.ContainsKey(key) && double.TryParse(stored.Metadata[key], out value);
        }
    }
}
