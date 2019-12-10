using InDoOut_Core.Entities.Functions;
using InDoOut_Display_Core.Elements;
using System;

namespace InDoOut_Display_Core.Functions
{
    /// <summary>
    /// A function that is capable of updating an associated <see cref="IElement"/> when triggered.
    /// </summary>
    public abstract class ElementFunction : Function, IElementFunction
    {
        public abstract IElement CreateAssociatedElement();

        protected override IOutput Started(IInput triggeredBy)
        {
            throw new NotImplementedException();
        }
    }
}
