using InDoOut_Core.Entities.Functions;
using InDoOut_Display_Core.Elements;
using System;

namespace InDoOut_Display_Core.Functions
{
    /// <summary>
    /// A function that is capable of updating an associated <see cref="IDisplayElement"/> when triggered.
    /// </summary>
    public abstract class ElementFunction : Function, IElementFunction
    {
        private DateTime _lastUIUpdate = DateTime.MinValue;

        /// <summary>
        /// Whether an update of the interface is required due to a change on the contained data.
        /// </summary>
        public bool ShouldDisplayUpdate => HasCompletedSince(_lastUIUpdate);

        /// <summary>
        /// Creats a <see cref="IDisplayElement"/> which can be associated with this function.
        /// </summary>
        /// <returns>A <see cref="IDisplayElement"/> that can be associated with and update from this function.</returns>
        public abstract IDisplayElement CreateAssociatedUIElement();

        /// <summary>
        /// Should be called when an update on the UI has taken place, in order to
        /// reset <see cref="ShouldDisplayUpdate"/>.
        /// </summary>
        public void PerformedUIUpdate()
        {
            _lastUIUpdate = DateTime.Now;
        }

        /// <summary>
        /// Called when this function should be started.
        /// </summary>
        /// <param name="triggeredBy">The input that triggered this function.</param>
        /// <returns>The output to call after this function completes.</returns>
        protected override IOutput Started(IInput triggeredBy)
        {
            //Todo: This might need some functionality

            return null;
        }
    }
}
