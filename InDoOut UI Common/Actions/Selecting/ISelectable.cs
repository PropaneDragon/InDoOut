namespace InDoOut_UI_Common.Actions.Selecting
{
    public interface ISelectable
    {
    }

    public interface ISelectable<ViewType> : ISelectable
    {
        public void SelectionStarted(ViewType view);
        public void SelectionEnded(ViewType view);

        public bool CanSelect(ViewType view);
    }
}
