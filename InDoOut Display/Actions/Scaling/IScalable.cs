using InDoOut_Display.UI.Controls.Screens;

namespace InDoOut_Display.Actions.Scaling
{
    public interface IScalable
    {
        bool AutoScale { get; set; }
        double Scale { get; set; }

        bool CanScale(IScreen screen);
        void ScaleChanged(IScreen screen);
    }
}
