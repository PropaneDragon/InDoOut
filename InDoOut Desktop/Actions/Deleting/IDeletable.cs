using InDoOut_Desktop.UI.Interfaces;

namespace InDoOut_Desktop.Actions.Deleting
{
    public interface IDeletable
    {
        void Deleted(IBlockView blockView);

        bool CanDelete(IBlockView blockView);
    }
}
