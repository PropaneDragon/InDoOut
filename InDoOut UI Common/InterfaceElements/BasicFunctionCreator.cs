using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.Controls.CoreEntityRepresentation;
using System.Windows;

namespace InDoOut_UI_Common.InterfaceElements
{
    public class BasicFunctionCreator : AbstractCreator, IFunctionCreator
    {
        public virtual IUIFunction Create(ICommonProgramDisplay display, IFunction function, bool setPositionFromMetadata = true)
        {
            var program = display?.AssociatedProgram;
            if (program != null && function != null)
            {
                if (program.Functions.Contains(function) || program.AddFunction(function))
                {
                    return AddFunctionToDisplay(function, display, setPositionFromMetadata);
                }
            }

            return null;
        }

        protected virtual IUIFunction AddFunctionToDisplay(IFunction function, ICommonProgramDisplay display, bool setPositionFromMetadata = true) => AddBasicUIFunctionToDisplay(function, display, setPositionFromMetadata);

        protected IUIFunction AddBasicUIFunctionToDisplay(IFunction function, ICommonProgramDisplay display, bool setPositionFromMetadata = true)
        {
            if (function != null)
            {
                var location = display?.CentreViewCoordinate ?? new Point();

                if (setPositionFromMetadata && ExtractPointFromMetadata(function, out var extractedLocation))
                {
                    location = extractedLocation;
                }

                var uiFunction = new UIFunction(function);

                display?.Add(uiFunction, location);

                return uiFunction;
            }

            return null;
        }
    }
}
