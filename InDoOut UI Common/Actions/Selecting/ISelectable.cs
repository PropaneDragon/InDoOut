using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Actions.Selecting
{
    public interface ISelectable
    {
        public void SelectionStarted(IElementDisplay view);
        public void SelectionEnded(IElementDisplay view);

        public bool CanSelect(IElementDisplay view);
    }
}
