namespace InDoOut_Core.Entities.Core
{
    /// <summary>
    /// Represents a base entity. All entities are <see cref="IStored"/>, so can
    /// be saved.
    /// </summary>
    public interface IEntity : IStored
    {
    }
}
