namespace InDoOut_Core.Basic
{
    /// <summary>
    /// Represents something that can have a name.
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// Name of the entity.
        /// </summary>
        string Name { get; }
    }
}
