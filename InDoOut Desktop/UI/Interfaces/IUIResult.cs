using InDoOut_Core.Entities.Functions;

namespace InDoOut_Desktop.UI.Interfaces
{
    public interface IUIResult : IUIConnectionStart
    {
        IResult AssociatedResult { get; set; }
    }
}
