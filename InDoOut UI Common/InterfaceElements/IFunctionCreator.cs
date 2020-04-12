using InDoOut_Core.Entities.Functions;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IFunctionCreator
    {
        IUIFunction Create(ICommonProgramDisplay display, IFunction function, bool setPositionFromMetadata = true);
    }
}
