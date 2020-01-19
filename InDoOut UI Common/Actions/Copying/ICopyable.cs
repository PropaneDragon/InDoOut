namespace InDoOut_UI_Common.Actions.Copying
{
    public interface ICopyable<ViewType>
    {
        bool CopyTo(ICopyable<ViewType> other);
        bool CanCopy(ViewType view);

        ICopyable<ViewType> CreateCopy(ViewType view);
    }
}
