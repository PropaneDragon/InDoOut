using InDoOut_Core.Entities.Functions;
using InDoOut_Display_Core.Functions;
using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Creation;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_Display.Creation
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
                var associatedScreen = screenConnections?.CurrentScreen;
                if (associatedScreen != null)
                {
                    return associatedScreen?.DisplayElementCreator?.Create(function, setPositionFromMetadata);
                }
            }

            return null;
        }
    }
}
