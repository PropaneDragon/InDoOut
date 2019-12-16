using InDoOut_Display.UI.Controls.Screens;

namespace InDoOut_Display.Actions.Resizing
{
    public interface IResizable
    {
        void ResizeStarted(IScreen screen);
        void ResizeEnded(IScreen screen);

        bool CanResize(IScreen screen);
    }
}
