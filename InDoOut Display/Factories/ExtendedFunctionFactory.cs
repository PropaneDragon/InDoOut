using InDoOut_Core.Entities.Functions;
using InDoOut_Display.UI.Controls.DisplayElement;
using InDoOut_Display_Core.Functions;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Factories;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Display.Factories
{
    internal class ExtendedFunctionFactory : BasicFunctionFactory
    {
        protected override IUIFunction AddFunctionToDisplay(IFunction function, ICommonProgramDisplay display, bool setPositionFromMetadata = true)
        {
            if (function is IElementFunction elementFunction)
            {
                return AddElementFunctionToDisplay(elementFunction, display, setPositionFromMetadata);
            } 

            return base.AddFunctionToDisplay(function, display, setPositionFromMetadata);
        }

        protected IUIFunction AddElementFunctionToDisplay(IElementFunction function, ICommonProgramDisplay display, bool setPositionFromMetadata = true)
        {
            if (function != null && display is IScreenConnections screenConnections)
            {
                var margins = new Thickness(5);

                if (setPositionFromMetadata && ExtractThicknessFomMetadata(function, out var extractedThickness))
                {
                    margins = extractedThickness;
                }

                var displayElement = function.CreateAssociatedUIElement();
                var elementContainer = new DisplayElementContainer(displayElement) { MarginPercentages = margins, DisplayMode = display.CurrentViewMode == ProgramViewMode.IO ? UIFunctionDisplayMode.IO : UIFunctionDisplayMode.Variables };

                screenConnections?.CurrentScreen?.Add(elementContainer);

                return elementContainer;
            }

            return null;
        }
    }
}
