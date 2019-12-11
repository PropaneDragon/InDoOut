using InDoOut_Display_Core.Functions;
using System;
using System.Windows;

namespace InDoOut_Display_Core.Elements
{
    public interface IElement
    {
        bool ShouldDisplayUpdate { get; }
        Type AssociatedFunctionType { get; }

        bool CanAssociateWithFunction(IElementFunction function);
        bool TryAssociateWithFunction(IElementFunction function);

        UIElement CreateAssociatedUIElement();
    }

    public interface IElement<T> : IElement where T : class, IElementFunction
    {
        T AssociatedFunction { get; set; }
    }
}
