using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Creation
{
    public interface IFunctionCreator : IElementCreator
    {
        ICommonProgramDisplay Display { get; }

        IUIFunction Create(IFunction function, bool setPositionFromMetadata = true);
    }
}
