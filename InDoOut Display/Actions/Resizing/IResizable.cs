using InDoOut_Display.UI.Controls.Screens;

namespace InDoOut_Display.Actions.Resizing
{
    public interface IResizable
    {
        void ResizeStarted(IScreenItem screen);
        void ResizeEnded(IScreenItem screen);

        bool CanResize(IScreenItem screen);
    }
}
