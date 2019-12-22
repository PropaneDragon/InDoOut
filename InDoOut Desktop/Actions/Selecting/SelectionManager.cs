using InDoOut_Desktop.UI.Interfaces;
using InDoOut_UI_Common.Actions.Selecting;

namespace InDoOut_Desktop.Actions.Selecting
{
    internal class SelectionManager : AbstractSelectionManager<IBlockViewSelectable>
    {
        public IBlockView _associatedBlockView = null;

        public SelectionManager(IBlockView blockView) : base()
        {
            _associatedBlockView = blockView;
        }

        protected override void NotifySelectableEnded(IBlockViewSelectable selectable)
        {
            selectable.SelectionEnded(_associatedBlockView);
        }

        protected override void NotifySelectableStarted(IBlockViewSelectable selectable)
        {
            selectable.SelectionStarted(_associatedBlockView);
        }
    }
}
