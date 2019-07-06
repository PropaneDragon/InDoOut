namespace InDoOut_Core.Core
{
    /// <summary>
    /// Represents a base entity. All entities are <see cref="IStored"/>, so can
    /// be saved.
    /// </summary>
    interface IEntity : IStored
    {
    }
}
