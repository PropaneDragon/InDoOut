using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Actions.Copying
{
    public interface ICopyable
    {
        bool CopyTo(ICopyable other);
        bool CanCopy(IElementDisplay view);

        ICopyable CreateCopy(IElementDisplay view);
    }
}
