using InDoOut_Core.Entities.Functions;
using InDoOut_Executable_Core.History;
using InDoOut_UI_Common.Controls.CoreEntityRepresentation;
using InDoOut_UI_Common.InterfaceElements;
using System.Windows;

namespace InDoOut_UI_Common.Creation
{
    public class BasicFunctionCreator : AbstractElementCreator, IFunctionCreator
    {
        public ICommonProgramDisplay Display { get; private set; } = null;

        private BasicFunctionCreator()
        {
        }

        public BasicFunctionCreator(ICommonProgramDisplay display)
        {
            Display = display;
        }

        public virtual IUIFunction Create(IFunction function, bool setPositionFromMetadata = true)
        {
            var program = Display?.AssociatedProgram;
            if (program != null && function != null)
            {
                if (program.Functions.Contains(function) || program.AddFunction(function))
                {
                    var uiFunction = AddFunctionToDisplay(function, setPositionFromMetadata);
                    if (uiFunction != null)
                    {
                        _ = History.Instance.AddHistory("Added function to project", () => AddFunctionToDisplay(function, setPositionFromMetadata), () => Display?.DeletableRemover.TryRemove(uiFunction), false);

                        return uiFunction;
                    }
                }
            }

            return null;
        }

        protected virtual IUIFunction AddFunctionToDisplay(IFunction function, bool setPositionFromMetadata = true) => AddBasicUIFunctionToDisplay(function, setPositionFromMetadata);

        protected IUIFunction AddBasicUIFunctionToDisplay(IFunction function, bool setPositionFromMetadata = true)
        {
            if (function != null && Display != null)
            {
                var location = Display?.CentreViewCoordinate ?? new Point();

                if (setPositionFromMetadata && ExtractPointFromMetadata(function, out var extractedLocation))
                {
                    location = extractedLocation;
                }

                var uiFunction = new UIFunction(function);

                Display?.Add(uiFunction, location);

                return uiFunction;
            }

            return null;
        }
    }
}
