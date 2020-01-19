namespace InDoOut_UI_Common.Actions.Deleting
{
    public interface IDeletable<ViewType>
    {
        void Deleted(ViewType view);

        bool CanDelete(ViewType view);
    }
}
