using InDoOut_Desktop.UI.Interfaces;

namespace InDoOut_Desktop.Actions.Copying
{
    public interface ICopyable
    {
        bool CopyTo(ICopyable other);
        bool CanCopy(IBlockView blockView);

        ICopyable CreateCopy(IBlockView blockView);
    }
}
