using System;
using InDoOut_Display_Core.Functions;

namespace InDoOut_Display_Core.Elements
{
    public abstract class Element<T> : IElement<T> where T : class, IElementFunction
    {
        private T _associatedFunction = null;
        private DateTime _lastUpdateTime = DateTime.MinValue;

        protected bool ShouldDisplayUpdate => _associatedFunction?.HasCompletedSince(_lastUpdateTime) ?? false;

        public T AssociatedFunction { get => _associatedFunction; set => ChangeFunction(value); }

        public Type AssociatedFunctionType => typeof(T);

        public bool CanAssociateWithFunction(IElementFunction function)
        {
            return AssociatedFunctionType.IsAssignableFrom(function.GetType());
        }

        public bool TryAssociateWithFunction(IElementFunction function)
        {
            if (CanAssociateWithFunction(function) && function is T correctTypeFunction)
            {
                AssociatedFunction = correctTypeFunction;

                return true;
            }

            return false;
        }

        protected void PerformedAnUpdate()
        {
            _lastUpdateTime = DateTime.Now;
        }

        protected virtual void FunctionChanged(T function) { };

        private void ChangeFunction(T function)
        {
            _associatedFunction = function;
            _lastUpdateTime = DateTime.MinValue;

            FunctionChanged(function);
        }
    }
}
