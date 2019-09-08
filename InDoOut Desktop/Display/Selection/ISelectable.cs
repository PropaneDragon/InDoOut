using InDoOut_Desktop.UI.Interfaces;

namespace InDoOut_Desktop.Display.Selection
{
    public interface ISelectable
    {
        public void SelectionStarted(IBlockView view);
        public void SelectionEnded(IBlockView view);

        public bool CanSelect(IBlockView view);
    }
}
