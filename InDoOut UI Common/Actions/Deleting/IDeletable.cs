using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Actions.Deleting
{
    public interface IDeletable
    {
        void Deleted(IElementDisplay view);

        bool CanDelete(IElementDisplay view);
    }
}
