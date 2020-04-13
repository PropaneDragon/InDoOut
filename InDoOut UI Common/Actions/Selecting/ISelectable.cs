using InDoOut_UI_Common.InterfaceElements;

namespace InDoOut_UI_Common.Actions.Selecting
{
    public interface ISelectable
    {
        public void SelectionStarted(ICommonDisplay display);
        public void SelectionEnded(ICommonDisplay display);

        public bool CanSelect(ICommonDisplay display);
    }
}
