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
        private DateTime _lastUIUpdate = DateTime.MinValue;

        public bool ShouldDisplayUpdate => HasCompletedSince(_lastUIUpdate);

        public abstract IDisplayElement CreateAssociatedUIElement();

        public void PerformedUIUpdate()
        {
            _lastUIUpdate = DateTime.Now;
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            //Todo: This might need some functionality

            return null;
        }
    }
}
