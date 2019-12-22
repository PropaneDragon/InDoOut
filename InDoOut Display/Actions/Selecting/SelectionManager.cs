using InDoOut_Display.UI.Controls.Screens;
using InDoOut_UI_Common.Actions.Selecting;

namespace InDoOut_Display.Actions.Selecting
{
    internal class SelectionManager : AbstractSelectionManager<IScreenSelectable>
    {
        private readonly IScreen _associatedScreen = null;

        public SelectionManager(IScreen screen)
        {
            _associatedScreen = screen;
        }

        protected override void NotifySelectableEnded(IScreenSelectable selectable)
        {
            selectable.SelectionEnded(_associatedScreen);
        }

        protected override void NotifySelectableStarted(IScreenSelectable selectable)
        {
            selectable.SelectionStarted(_associatedScreen);
        }
    }
}
