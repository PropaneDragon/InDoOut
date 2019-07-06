namespace InDoOut_Core.Core
{
    /// <summary>
    /// Represents something that can have a name.
    /// </summary>
    interface INamed
    {
        /// <summary>
        /// Name of the entity.
        /// </summary>
        string Name { get; }
    }
}
