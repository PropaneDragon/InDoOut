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

                if (program.Metadata.ContainsKey("x") && program.Metadata.ContainsKey("y") && double.TryParse(program.Metadata["x"], out var screenLeft) && double.TryParse(program.Metadata["y"], out var screenTop) && _associatedBlockView is IScrollable scrollable)
                {
                    scrollable.Offset = new Point(screenLeft, screenTop);
                }

                foreach (var function in program.Functions)
                {
                    if (function.Metadata.ContainsKey("x") && function.Metadata.ContainsKey("y") && double.TryParse(function.Metadata["x"], out var xPosition) && double.TryParse(function.Metadata["y"], out var yPosition))
                    {
                        var uiFunction = _associatedBlockView.Create(function, new Point(xPosition, yPosition));
                        if (uiFunction != null)
                        {
                            functionToUIFunctionMap.Add(function, uiFunction);
                        }
                    }
                    else
                    {
                        var uiFunction = _associatedBlockView.Create(function);
                        if (uiFunction != null)
                        {
                            functionToUIFunctionMap.Add(function, uiFunction);
                        }
                    }
                }

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
                                        var connection = _associatedBlockView.Create(uiOutput, uiInput);

                                        if (ExtractMetadataValue(output, "x", out var outputX) && ExtractMetadataValue(output, "y", out var outputY) && ExtractMetadataValue(output, "w", out var outputW) && ExtractMetadataValue(output, "h", out var outputH) &&
                                            ExtractMetadataValue(input, "x", out var inputX) && ExtractMetadataValue(input, "y", out var inputY) && ExtractMetadataValue(input, "w", out var inputW) && ExtractMetadataValue(input, "h", out var inputH))
                                        {
                                            var outputArea = new Rect(outputX, outputY, outputW, outputH);
                                            var inputArea = new Rect(inputX, inputY, inputW, inputH);
                                            var outputCentre = outputArea.TopLeft + ((outputArea.BottomRight - outputArea.TopLeft) / 2d);
                                            var inputCentre = inputArea.TopLeft + ((inputArea.BottomRight - inputArea.TopLeft) / 2d);

                                            connection.Start = _associatedBlockView.GetBestSide(outputArea, inputCentre);
                                            connection.End = _associatedBlockView.GetBestSide(inputArea, outputCentre);
                                        }
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
                                        var connection = _associatedBlockView.Create(uiResult, uiProperty);

                                        if (ExtractMetadataValue(result, "x", out var outputX) && ExtractMetadataValue(result, "y", out var outputY) && ExtractMetadataValue(result, "w", out var outputW) && ExtractMetadataValue(result, "h", out var outputH) &&
                                            ExtractMetadataValue(property, "x", out var inputX) && ExtractMetadataValue(property, "y", out var inputY) && ExtractMetadataValue(property, "w", out var inputW) && ExtractMetadataValue(property, "h", out var inputH))
                                        {
                                            var outputArea = new Rect(outputX, outputY, outputW, outputH);
                                            var inputArea = new Rect(inputX, inputY, inputW, inputH);
                                            var outputCentre = outputArea.TopLeft + ((outputArea.BottomRight - outputArea.TopLeft) / 2d);
                                            var inputCentre = inputArea.TopLeft + ((inputArea.BottomRight - inputArea.TopLeft) / 2d);

                                            connection.Start = _associatedBlockView.GetBestSide(outputArea, inputCentre);
                                            connection.End = _associatedBlockView.GetBestSide(inputArea, outputCentre);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        public bool UnloadProgram(IProgram program)
        {
            if (program != null)
            {
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
