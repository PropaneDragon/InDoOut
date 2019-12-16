using InDoOut_Display.UI.Controls.Screens;

namespace InDoOut_Display.Actions
{
    internal class ScreenRestingAction : InDoOut_UI_Common.Actions.Action
    {
        private IScreen _screen = null;

        public ScreenRestingAction(IScreen screen)
        {
            _screen = screen;
        }
    }
}
