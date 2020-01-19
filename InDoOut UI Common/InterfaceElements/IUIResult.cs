using InDoOut_Core.Entities.Functions;

namespace InDoOut_UI_Common.InterfaceElements
{
    public interface IUIResult : IUIConnectionStart
    {
        IResult AssociatedResult { get; set; }
    }
}
