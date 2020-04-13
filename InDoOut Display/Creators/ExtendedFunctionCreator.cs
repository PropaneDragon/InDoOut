using InDoOut_Core.Entities.Functions;
using InDoOut_Display.UI.Controls.DisplayElement;
using InDoOut_Display_Core.Functions;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Creators;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Display.Creators
{
    internal class ExtendedFunctionCreator : BasicFunctionCreator
    {
        public ExtendedFunctionCreator(ICommonProgramDisplay display) : base(display)
        {
        }

        protected override IUIFunction AddFunctionToDisplay(IFunction function, bool setPositionFromMetadata = true)
        {
            return function is IElementFunction elementFunction ? AddElementFunctionToDisplay(elementFunction, setPositionFromMetadata) : base.AddFunctionToDisplay(function, setPositionFromMetadata);
        }

        protected IUIFunction AddElementFunctionToDisplay(IElementFunction function, bool setPositionFromMetadata = true)
        {
            if (function != null && Display is IScreenConnections screenConnections)
            {
                var margins = new Thickness(5);

                if (setPositionFromMetadata && ExtractThicknessFomMetadata(function, out var extractedThickness))
                {
                    margins = extractedThickness;
                }

                var displayElement = function.CreateAssociatedUIElement();
                var elementContainer = new DisplayElementContainer(displayElement) { MarginPercentages = margins, DisplayMode = Display.CurrentViewMode == ProgramViewMode.IO ? UIFunctionDisplayMode.IO : UIFunctionDisplayMode.Variables };

                screenConnections?.CurrentScreen?.Add(elementContainer);

                return elementContainer;
            }

            return null;
        }
    }
}
