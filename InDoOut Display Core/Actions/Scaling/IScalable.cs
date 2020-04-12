using InDoOut_Display_Core.Screens;

namespace InDoOut_Display_Core.Actions.Scaling
{
    /// <summary>
    /// Represents something that has contents that can be scaled.
    /// </summary>
    public interface IScalable
    {
        /// <summary>
        /// Whether or not to handle scaling automatically or manually.
        /// If <see cref="AutoScale"/> is false <see cref="Scale"/> is used.
        /// </summary>
        bool AutoScale { get; set; }

        /// <summary>
        /// The multiplier to apply to the scale. Only available if
        /// <see cref="AutoScale"/> is false.
        /// </summary>
        double Scale { get; set; }

        /// <summary>
        /// Whether or not the given <see cref="IScalable"/> can actually
        /// scale.
        /// </summary>
        /// <param name="screen">The <see cref="IScreen"/> to check scaling agains.</param>
        /// <returns>Whether or not this element is allowed to scale.</returns>
        bool CanScale(IScreen screen);

        /// <summary>
        /// Triggered when the scale of the <see cref="IScalable"/> has changed in order
        /// to do some post processing.
        /// </summary>
        /// <param name="screen">The screen by which the scaling has been applied from.</param>
        void ScaleChanged(IScreen screen);
    }
}
