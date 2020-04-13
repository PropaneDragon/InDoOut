using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Factories
{
    public interface IFunctionFactory
    {
        IUIFunction Create(IFunction function, bool setPositionFromMetadata = true);
    }
}
