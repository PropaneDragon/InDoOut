using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Creators
{
    public interface IFunctionCreator
    {
        IUIFunction Create(IFunction function, bool setPositionFromMetadata = true);
    }
}
