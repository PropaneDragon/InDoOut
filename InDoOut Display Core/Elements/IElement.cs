using InDoOut_Display_Core.Functions;
using System;

namespace InDoOut_Display_Core.Elements
{
    public interface IElement
    {
        Type AssociatedFunctionType { get; }

        bool CanAssociateWithFunction(IElementFunction function);
        bool TryAssociateWithFunction(IElementFunction function);
    }

    public interface IElement<T> : IElement where T : class, IElementFunction
    {
        T AssociatedFunction { get; set; }
    }
}
