using InDoOut_Core.Threading.Safety;
using InDoOut_Display.UI.Controls.DisplayElement;
using InDoOut_Display_Core.Creation;
using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Creation;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_Display.Creation
{
    internal class DisplayElementCreator : AbstractElementCreator, IDisplayElementCreator
    {
        protected IScreen Screen { get; private set; } = null;

        private DisplayElementCreator()
        {
        }

        public DisplayElementCreator(IScreen screen)
        {
            Screen = screen;
        }

        public IDisplayElementContainer Create(IElementFunction elementFunction, bool setSizeFromMetadata = true)
        {
            var displayElement = TryGet.ValueOrDefault(() => elementFunction?.CreateAssociatedUIElement());
            return displayElement != null ? Create(displayElement, setSizeFromMetadata) : null;
        }

        public IDisplayElementContainer Create(IDisplayElement displayElement, bool setSizeFromMetadata = true)
        {
            var associatedProgram = Screen?.AssociatedProgram;
            var associatedElementFunction = displayElement?.AssociatedElementFunction;

            if (associatedProgram != null && associatedElementFunction != null && (associatedProgram.Functions.Contains(associatedElementFunction) || associatedProgram.AddFunction(associatedElementFunction)))
            {
                var margins = new Thickness(0.1);

                if (setSizeFromMetadata && ExtractThicknessFomMetadata(associatedElementFunction, out var extractedMargins))
                {
                    margins = extractedMargins;
                }

                var container = new DisplayElementContainer(displayElement) { DisplayMode = Screen?.CurrentViewMode == ProgramViewMode.IO ? UIFunctionDisplayMode.IO : UIFunctionDisplayMode.Variables, MarginPercentages = margins };

                Screen?.Add(container);

                return container;
            }

            return null;
        }
    }
}
