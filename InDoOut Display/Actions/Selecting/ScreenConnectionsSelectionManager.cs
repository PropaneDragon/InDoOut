using InDoOut_Display_Core.Screens;
using InDoOut_UI_Common.Actions.Selecting;

namespace InDoOut_Display.Actions.Selecting
{
    internal class ScreenConnectionsSelectionManager : AbstractSelectionManager<ISelectable>
    {
        private readonly IScreenConnections _associatedScreen = null;

        public ScreenConnectionsSelectionManager(IScreenConnections screenConnections)
        {
            _associatedScreen = screenConnections;
        }

        protected override void NotifySelectableEnded(ISelectable selectable)
        {
            selectable.SelectionEnded(_associatedScreen);
        }

        protected override void NotifySelectableStarted(ISelectable selectable)
        {
            selectable.SelectionStarted(_associatedScreen);
        }
    }
}
